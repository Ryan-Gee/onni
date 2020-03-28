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
			var CommentsCount = _context.Comments.Where(y => y.CommentDate.Month == DateTime.Now.Month).Count();
			var ProjectsCount = _context.Projects.Where(p => p.CreatedDate.Month == DateTime.Now.Month).Count();
			var MostLiked = _context.Projects.OrderByDescending(p => p.LikeCounts).Take(5).ToList();
			var MostViewed = _context.Projects.OrderByDescending(p => p.ViewCounts).Take(5).ToList();
			var PendingProjects = _context.Projects.Where(p => p.StatusId == 1).Count();
			var ProjectsInYears = _context.Projects.Where(p => p.CreatedDate.Year == 2020).GroupBy(p => p.CreatedDate.Month)
				.Select(g => new ProjectsInYear { Mouth = g.Key, Count = g.Count() })
				.ToList().OrderBy(s => s.Mouth);
			var CommentsInYear = _context.Comments.Where(p => p.CommentDate.Year == 2020).GroupBy(p => p.CommentDate.Month)
				.Select(g => new CommentsInYear { Mouth = g.Key, Count = g.Count() })
				.ToList().OrderBy(s => s.Mouth);

			ViewData["CommentsCount"] = CommentsCount;
			ViewData["ProjectsCount"] = ProjectsCount;
			ViewData["MostLiked"] = MostLiked; 
			ViewData["MostViewed"] = MostViewed;
			ViewData["PendingProjects"] = PendingProjects;
			ViewData["ProjectsInYears"] = ProjectsInYears;
			ViewData["CommentsInYear"] = CommentsInYear;

			return View();
		}
	}

}

