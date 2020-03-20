using System;
using System.Collections.Generic;
using System.IO;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using onni.Models;

namespace onni.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		//Used for file upload
		private readonly IHostingEnvironment hostingEnvironment;
		//Add db context
		private readonly ChangeMakingContext _context;

		public AdminController(ChangeMakingContext context, IHostingEnvironment environment)
		{
			_context = context;
			hostingEnvironment = environment;
		}

		// GET: Projects
		public async Task<IActionResult> Index()
		{
			var changeMakingContext = _context.Projects.Include(p => p.Category).Include(p => p.ParentProject).Include(p => p.Status);
			return View(await changeMakingContext.ToListAsync());
		}



		public async Task<IActionResult> Public(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comments = await _context.Comments
				.Include(c => c.Project)
				.FirstOrDefaultAsync(m => m.CommentId == id);
			if (comments == null)
			{
				return NotFound();
			}

			return View(comments);
		}

		// POST: Make a project public
		[HttpPost]
		[ValidateAntiForgeryToken]
		public  IActionResult Public(int id)
		{
			var project = _context.Projects.SingleOrDefault(m => m.ProjectId == id);
			// projectID 1: Pending 2:Draft 3:Public
			project.StatusId = 3;
			_context.Update(project);
			_context.SaveChanges();
			return RedirectToAction ("index");
		}


		// GET report 
		public IActionResult Report()
		{

			var commentsInYear = _context.Comments.Where(y => y.CommentDate.Year == 2020).Count();
			var ProjectsCount = _context.Projects.Where(p => p.CreatedDate.Month == 3).Count();
			var MostLiked = _context.Projects.OrderByDescending(p => p.LikeCounts).Take(5).ToList();
			var MostViewed = _context.Projects.OrderByDescending(p => p.ViewCounts).Take(5).ToList();
			var PendingProjects = _context.Projects.Where(p => p.StatusId == 1).Count();
			var ProjectsInYears = _context.Projects.Where(p => p.CreatedDate.Year == 2020).GroupBy(p => p.CreatedDate.Month)
				//.Select(g => new ProjectsInYear { Mouth = g.Key, Count = g.Count() })
				.ToList();

			ViewData["commentsInYear"] = commentsInYear;
			ViewData["ProjectsCount"] = ProjectsCount;
			ViewData["MostLiked"] = MostLiked; 
			ViewData["MostViewed"] = MostViewed;
			ViewData["PendingProjects"] = PendingProjects;
			ViewData["ProjectsInYears"] = ProjectsInYears;

			return View(ProjectsInYears);
		}





		// GET: Projects/Create
		public IActionResult Create(int? ParentProjectId)
		//passing ParentProjectId when create child project
		{
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName");
			ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName");
			ViewBag.ParentProjectId = ParentProjectId;
			return View(new ProjectUpload());
		}

		// POST: Projects/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProjectUpload projectUpload)
		{
			//Validate according to the upload model
			if (ModelState.IsValid)
			{
				//Create a project model and bind everything from upload to the new model
				var project = new Projects();
				project.ProjectName = projectUpload.ProjectName;
				project.UserName = User.Identity.Name;
				project.CreatedDate = DateTime.Now;
				project.BodyContent = projectUpload.BodyContent;
				project.Files = projectUpload.Files;
				project.Images = projectUpload.Images;
				project.ViewCounts = 0;
				project.LikeCounts = 0;
				project.StatusId = 1;
				project.ParentProjectId = projectUpload.ParentProjectId;
				project.Tags = projectUpload.Tags;
				project.CategoryId = projectUpload.CategoryId;

				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", projectUpload.CategoryId);
			//ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", projectUpload.ParentProjectId);
			ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", projectUpload.StatusId);
			return View("index");
		}


		// GET: Projects/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var projects = await _context.Projects.FindAsync(id);
			if (projects == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", projects.CategoryId);
			ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", projects.ParentProjectId);
			ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", projects.StatusId);
			return View(projects);
		}

		// POST: Projects/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjectName,UserName,CreatedDate,BodyContent,Files,Images,ViewCounts,LikeCounts,StatusId,ParentProjectId,Tags,CategoryId")] Projects projects)
		{
			if (id != projects.ProjectId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(projects);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectsExists(projects.ProjectId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", projects.CategoryId);
			ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", projects.ParentProjectId);
			ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", projects.StatusId);
			return View(projects);
		}

		// GET: Projects/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var projects = await _context.Projects
				.Include(p => p.Category)
				.Include(p => p.ParentProject)
				.Include(p => p.Status)
				.FirstOrDefaultAsync(m => m.ProjectId == id);
			if (projects == null)
			{
				return NotFound();
			}

			return View(projects);
		}

		// POST: Projects/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var projects = await _context.Projects.FindAsync(id);
			_context.Projects.Remove(projects);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectsExists(int id)
		{
			return _context.Projects.Any(e => e.ProjectId == id);
		}


	}

}

