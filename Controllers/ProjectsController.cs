using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onni.Models;

namespace onni.Controllers
{
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
		public async Task<IActionResult> Index()
        {
            var changeMakingContext = _context.Projects.Include(p => p.Category).Include(p => p.ParentProject).Include(p => p.Status);
            return View(await changeMakingContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName");
            ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName");
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName");
            return View(new ProjectUpload());
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectUpload upload)
        {
			//Validate according to the upload model
            if (ModelState.IsValid)
            {
				//Create a project model and bind everything from upload to the new model
				var project = new Projects();
				project.ProjectName = upload.ProjectName;
				project.UserName = User.Identity.Name;
                //Get Date time now for CreateTime
                project.CreatedDate = DateTime.Now;
                project.BodyContent = upload.BodyContent;

                // initialize default value 
				project.ViewCounts = 0;
				project.LikeCounts = 0;
                // 1: pending 2: draft 3:public
				project.StatusId = 1;

				project.ParentProjectId = upload.ParentProjectId;
				project.Tags = upload.Tags;
				project.CategoryId = upload.CategoryId;

				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoriesId", "CategoriesName", upload.CategoryId);
            ViewData["ParentProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", upload.ParentProjectId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusName", upload.StatusId);
            return View("index");
        }

		private string MakeFileNameUnique (string input)
		{
			input = Path.GetFileName(input);
			return Path.GetFileNameWithoutExtension(input) + "_" + Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(input);
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
