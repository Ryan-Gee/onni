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
			var changeMakingContext = _context.Projects.Include(p => p.Category).Include(p => p.ParentProject).Include(p => p.Status).OrderBy(p => p.CreatedDate);
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

			var thisYear = DateTime.Now.Year;

			var CommentsCount = _context.Comments.Where(y => y.CommentDate.Month == DateTime.Now.Month).Count();
			var ProjectsCount = _context.Projects.Where(p => p.CreatedDate.Month == DateTime.Now.Month).Count();
			var MostLiked = _context.Projects.OrderByDescending(p => p.LikeCounts).Take(5).ToList();
			var MostViewed = _context.Projects.OrderByDescending(p => p.ViewCounts).Take(5).ToList();
			var PendingProjects = _context.Projects.Where(p => p.StatusId == 1).Count();
			var ProjectsInYears = _context.Projects.Where(p => p.CreatedDate.Year == thisYear).GroupBy(p => p.CreatedDate.Month)
				.Select(g => new ProjectsInYear { Mouth = g.Key, Count = g.Count() })
				.ToList().OrderBy(s => s.Mouth);
			var CommentsInYear = _context.Comments.Where(p => p.CommentDate.Year == thisYear).GroupBy(p => p.CommentDate.Month)
				.Select(g => new CommentsInYear { Mouth = g.Key, Count = g.Count() })
				.ToList().OrderBy(s => s.Mouth);


			//Combine commentsInYear and projectsInYears into a single List
			//Usually this would be done with a single sql query instead of two and then merging, but .Net doesn't allow sql linq queries to search multiple collections in a single query

			//select count(distinct(p.projectname)) as projects, count(distinct(c.commentdate)) as comments from Projects as p, (select * from Comments) as c where month(p.CreatedDate) = 3 and month(c.commentdate) = 3

			var ActivityThisYear = new List<ActivityInYear>();
			for (var i = 1; i <= 12; i++)
			{
				var monthActivity = new ActivityInYear { Month = i, yearComments = 0, yearProjects = 0 };
				foreach (var p in ProjectsInYears)
				{
					if (p.Mouth == i)
					{
						monthActivity.yearProjects = p.Count;
					}
				}
				foreach (var c in CommentsInYear)
				{
					if (c.Mouth == i)
					{
						monthActivity.yearComments = c.Count;
					}
				}
				if (monthActivity.yearComments != 0 || monthActivity.yearProjects != 0)
				{
					ActivityThisYear.Add(monthActivity);
				}
			}

			var yearBest = new List<BestOfYear>();
			for (var i = 0; i < 5; i++)
			{
				var record = new BestOfYear();
				if (MostLiked.Count() > i)
				{
					record.likes = MostLiked[i];
				}
				if (MostViewed.Count() > i)
				{
					record.views = MostViewed[i];

				}
				yearBest.Add(record);
			}

			ViewData["CommentsCount"] = CommentsCount;
			ViewData["ProjectsCount"] = ProjectsCount;
			ViewData["MostLiked"] = MostLiked; 
			ViewData["MostViewed"] = MostViewed;
			ViewData["PendingProjects"] = PendingProjects;
			ViewData["ActivityInYear"] = ActivityThisYear;
			ViewData["YearRecords"] = yearBest;
			//ViewData["ProjectsInYears"] = ProjectsInYears;
			//ViewData["CommentsInYear"] = CommentsInYear;

			return View();
		}
	}

}

