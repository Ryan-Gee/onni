using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onni.Models;

namespace onni.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		private readonly ChangeMakingContext _context;

		public DashboardController(ChangeMakingContext context)
		{
			_context = context;
		}

		// GET: Projects
		//public async Task<IActionResult> Index()
		//{
		//	var changeMakingContext = _context.Projects.Include(p => p.Category).Include(p => p.ParentProject).Include(p => p.Status);
		//	return View(await changeMakingContext.ToListAsync());
		//}


		// GET 
		public IActionResult Index()
		{

			var MyProjects = _context.Projects.Where(p => p.UserName == User.Identity.Name).OrderByDescending(p => p.CreatedDate).ToList();
			var MyLiked = _context.SavedProjects.Include(p=>p.Project).Where(p => p.UserName == User.Identity.Name).OrderByDescending(p => p.SavedDate).ToList();
			var PendingProjects = _context.Projects.Where(p => p.StatusId == 1).Count();

			var myLikes = 0;
			var myViews = 0;
			var projCount = 0;
			foreach (var proj in MyProjects)
			{
				myLikes += proj.LikeCounts;
				myViews += proj.ViewCounts;
				projCount += 1;
			}


			ViewData["MyProjects"] = MyProjects;
			ViewData["MyLiked"] = MyLiked;
			ViewData["myLikes"] = myLikes;
			ViewData["myViews"] = myViews;
			ViewData["projCount"] = projCount;

			return View();
		}
	}

}

