﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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
	[Authorize]
	public class ProjectsController : Controller
	{
		//Used for file upload
		private readonly IHostingEnvironment hostingEnvironment;
		//Add db context
		private readonly ChangeMakingContext _context;

		public ProjectsController(ChangeMakingContext context, IHostingEnvironment environment)
		{
			_context = context;
			hostingEnvironment = environment;
		}

		// GET: Projects
		[AllowAnonymous]
		public async Task<IActionResult> Index(string searchString, string ProjectCategory)
		{
			// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/search?view=aspnetcore-2.1
			IQueryable<string> genreQuery = from m in _context.Projects
											orderby m.Category.CategoriesName
											select m.Category.CategoriesName;
			var projects = from p in _context.Projects.Include(p => p.Category).Include(p => p.ParentProject).Include(p => p.Status) select p;			
			if (!string.IsNullOrEmpty(searchString))
			{
				projects = projects.Where(s => s.ProjectName.Contains(searchString) || s.BodyContent.Contains(searchString) || s.Tags.Contains(searchString));
				ViewBag.searchString =  searchString;
			}
			if (!string.IsNullOrEmpty(ProjectCategory))
			{
				projects = projects.Where(x => x.Category.CategoriesName == ProjectCategory);
				ViewBag.ProjectCategory =  ProjectCategory;
			}

			var ProjectCategorie = new ProjectCategories
			{
				Categories = new SelectList(await genreQuery.Distinct().ToListAsync()),
				Projects = await projects.ToListAsync()
			};
			return View(ProjectCategorie);
		}

		// GET: Projects/Details/5
		// using comments model instead of project
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id)
		{
			{
				if (id == null)
				{
					return NotFound();
				}

				var projects = _context.Projects
					.Where(m => m.ProjectId == id)
					.Include(p => p.Category)
					.Include(p => p.ParentProject)
					.Include(p => p.Status)
					.Include(p => p.ViewCounts);
				if (projects == null)
				{
					return NotFound();
				}
				var savedProjects = _context.SavedProjects.SingleOrDefault(s => s.UserName == User.Identity.Name && s.ProjectId == id);
				if (savedProjects == null)
				{
					ViewBag.isLike = false;
				}
				else
				{
					ViewBag.isLike = true;
				}
				//Update view count
				var prj = _context.Projects.SingleOrDefault(p => p.ProjectId == id);
				if (prj != null)
				{
					prj.ViewCounts += 1;
					_context.SaveChanges();
				}
				// find all comments with the project ID
				var comments = _context.Comments.Where(p => p.ProjectId == id);
				ViewData["Projects"] = projects;
				ViewBag.id = id;
				return View(comments);
			}
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

		[HttpPost]
		public async Task<string> UploadImg(IFormFile file)
		{
			var uploads = Path.Combine(hostingEnvironment.WebRootPath, "upload/img");

			if (file.Length > 0)
			{
				var f = file.FileName;
				f = f.Replace(" ", "");
				f = f.Replace("/", "");
				f = f.Replace("\\", "");
				f = f.Replace("#", "");
				var fn = Guid.NewGuid() + "_" + Uri.EscapeUriString(f);
				using (var fileStream = new FileStream(Path.Combine(uploads, fn), FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
					return fn;
				}
			}
			else
			{
				return null;
			}
		}

		[HttpPost]
		public async Task<string> UploadFile(IFormFile file)
		{
			var uploads = Path.Combine(hostingEnvironment.WebRootPath, "upload/files");

			if (file.Length > 0)
			{
				var f = file.FileName;
				f = f.Replace(" ", "");
				f = f.Replace("/", "");
				f = f.Replace("\\", "");
				f = f.Replace("#", "");
				var fn = Guid.NewGuid() + "_" + Uri.EscapeUriString(f);
				using (var fileStream = new FileStream(Path.Combine(uploads, fn), FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
					return fn;
				}
			}
			else
			{
				return null;
			}
		}

		[HttpPost]
		public async Task<string> DeleteFile(string id)
		{
			var uploads = Path.Combine(hostingEnvironment.WebRootPath, "upload/files");
			var fn = id;

			var filepath = Path.Combine(uploads, fn);
			if (System.IO.File.Exists(filepath))
			{
				// If file found, delete it    
				System.IO.File.Delete(filepath);
				Console.WriteLine("File deleted.");
				return "";
			}
			else
			{
				return "File does not exist";
			}
		}

		[HttpPost]
		public async Task<string> DeleteImg(string id)
		{
			var uploads = Path.Combine(hostingEnvironment.WebRootPath, "upload/img");
			var fn = id;

			var filepath = Path.Combine(uploads, fn);
			if (System.IO.File.Exists(filepath))
			{
				// If file found, delete it    
				System.IO.File.Delete(filepath);
				Console.WriteLine("File deleted.");
				return "";
			}
			else
			{
				return "File does not exist";
			}
		}

		[HttpGet()]
		public IActionResult DownloadFile(string id)
		{
			var fileFolder = Path.Combine(hostingEnvironment.WebRootPath, "upload/files/");
			string path = fileFolder;
			byte[] fileBytes = System.IO.File.ReadAllBytes(path + id);
			string fileName = id.Substring(id.IndexOf("_") + 1);
			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		}

		//// GET: Projects/Edit/5
		//public async Task<IActionResult> Edit(int? id)
		//{
		//	if (id == null)
		//	{
		//		return NotFound();
		//	}

		//	var projects = await _context.Projects.FindAsync(id);
		//	if (projects == null)
		//	{
		//		return NotFound();
		//	}
		//	ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", projects.CategoryId);
		//	ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", projects.ParentProjectId);
		//	ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", projects.StatusId);
		//	return View(projects);
		//}

		//// POST: Projects/Edit/5
		//// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		//// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjectName,UserName,CreatedDate,BodyContent,Files,Images,ViewCounts,LikeCounts,StatusId,ParentProjectId,Tags,CategoryId")] Projects projects)
		//{
		//	if (id != projects.ProjectId)
		//	{
		//		return NotFound();
		//	}

		//	if (ModelState.IsValid)
		//	{
		//		try
		//		{
		//			_context.Update(projects);
		//			_context.SaveChanges();
		//			//await _context.SaveChangesAsync();
		//		}
		//		catch (DbUpdateConcurrencyException)
		//		{
		//			if (!ProjectsExists(projects.ProjectId))
		//			{
		//				return NotFound();
		//			}
		//			else
		//			{
		//				throw;
		//			}
		//		}
		//		return RedirectToAction(nameof(Index));
		//	}
		//	ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", projects.CategoryId);
		//	ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", projects.ParentProjectId);
		//	ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", projects.StatusId);
		//	return View(projects);
		//}

		// GET: Projects/Edit/5
		public async Task<IActionResult> Edit(int id)
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

			var upload = new ProjectUpload();
			upload.ProjectId = id;
			upload.ProjectName = projects.ProjectName;
			upload.UserName = projects.UserName;
			upload.CreatedDate = projects.CreatedDate;
			upload.BodyContent = projects.BodyContent;
			upload.Files = projects.Files;
			upload.Images = projects.Images;
			upload.ViewCounts = projects.ViewCounts;
			upload.LikeCounts = projects.LikeCounts;
			upload.StatusId = projects.StatusId;
			upload.ParentProjectId = projects.ParentProjectId;
			upload.Tags = projects.Tags;
			upload.CategoryId = projects.CategoryId;

			return View(upload);
		}

		// POST: Projects/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ProjectUpload projectUpload)
		{
			//Validate according to the upload model
			if (ModelState.IsValid)
			{
				//Create a project model and bind everything from upload to the new model
				var project = new Projects();
				project.ProjectId = id;
				project.ProjectName = projectUpload.ProjectName;
				project.UserName = projectUpload.UserName;
				project.CreatedDate = projectUpload.CreatedDate;
				project.BodyContent = projectUpload.BodyContent;
				project.Files = projectUpload.Files;
				project.Images = projectUpload.Images;
				project.ViewCounts = projectUpload.ViewCounts;
				project.LikeCounts = projectUpload.LikeCounts;
				project.StatusId = projectUpload.StatusId;
				project.ParentProjectId = projectUpload.ParentProjectId;
				project.Tags = projectUpload.Tags;
				project.CategoryId = projectUpload.CategoryId;

				_context.Update(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", projectUpload.CategoryId);
			//ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", projectUpload.ParentProjectId);
			ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", projectUpload.StatusId);
			return View("index");
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
			var imgList = projects.Images;
			var fileList = projects.Files;

			var images = imgList.Split(" ");
			var files = fileList.Split(" ");

			foreach (String img in images)
			{
				var uploads = Path.Combine(hostingEnvironment.WebRootPath, "upload/img");
				var fn = img;

				var filepath = Path.Combine(uploads, fn);
				if (System.IO.File.Exists(filepath))
				{
					// If file found, delete it    
					System.IO.File.Delete(filepath);
					Console.WriteLine("Image deleted.");
				}
				else
				{
					Console.WriteLine("Image failed to delete.");
				}
			}

			foreach (String f in files)
			{
				var uploads = Path.Combine(hostingEnvironment.WebRootPath, "upload/files/");
				var fn = f;

				var filepath = Path.Combine(uploads, fn);
				if (System.IO.File.Exists(filepath))
				{
					// If file found, delete it    
					System.IO.File.Delete(filepath);
					Console.WriteLine("File deleted.");
				}
				else
				{
					Console.WriteLine("File failed to delete.");
				}
			}

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

