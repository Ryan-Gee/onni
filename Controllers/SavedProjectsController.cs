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
    public class SavedProjectsController : Controller
    {
        private readonly ChangeMakingContext _context;

        public SavedProjectsController(ChangeMakingContext context)
        {
            _context = context;
        }

        // GET: SavedProjects
        public async Task<IActionResult> Index()
        {
            var changeMakingContext = _context.SavedProjects.Include(s => s.Project);
            return View(await changeMakingContext.ToListAsync());
        }

        // GET: SavedProjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedProjects = await _context.SavedProjects
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.SavedId == id);
            if (savedProjects == null)
            {
                return NotFound();
            }

            return View(savedProjects);
        }

        // GET: SavedProjects/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName");
            return View();
        }

        // POST: Like Button
        // Like
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Like( string UserName, int ProjectId)
        {
            var savedProjects = new SavedProjects
            {
                UserName = UserName,
                ProjectId = ProjectId,
                SavedDate = DateTime.Now
            };

                _context.SavedProjects.Add(savedProjects);
                _context.SaveChanges();

            return RedirectToAction("Details", "Projects", new { id = ProjectId });
        }
        // Unlike 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Unlike(string UserName, int ProjectId)
        {
            var savedProjects = _context.SavedProjects.SingleOrDefault(s => s.UserName == UserName && s.ProjectId == ProjectId);
            _context.SavedProjects.Remove(savedProjects);
            _context.SaveChanges();
            return RedirectToAction("Details", "Projects", new { id = ProjectId });
        }





        // POST: SavedProjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SavedId,UserName,ProjectId,SavedDate")] SavedProjects savedProjects)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savedProjects);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", savedProjects.ProjectId);
            return View(savedProjects);
        }

        // GET: SavedProjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedProjects = await _context.SavedProjects.FindAsync(id);
            if (savedProjects == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", savedProjects.ProjectId);
            return View(savedProjects);
        }

        // POST: SavedProjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SavedId,UserName,ProjectId,SavedDate")] SavedProjects savedProjects)
        {
            if (id != savedProjects.SavedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savedProjects);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavedProjectsExists(savedProjects.SavedId))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", savedProjects.ProjectId);
            return View(savedProjects);
        }

        // GET: SavedProjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedProjects = await _context.SavedProjects
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.SavedId == id);
            if (savedProjects == null)
            {
                return NotFound();
            }

            return View(savedProjects);
        }

        // POST: SavedProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var savedProjects = await _context.SavedProjects.FindAsync(id);
            _context.SavedProjects.Remove(savedProjects);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavedProjectsExists(int id)
        {
            return _context.SavedProjects.Any(e => e.SavedId == id);
        }
    }
}
