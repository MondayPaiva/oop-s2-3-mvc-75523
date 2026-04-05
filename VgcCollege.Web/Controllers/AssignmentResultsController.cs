using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null) return NotFound();

            return View(assignmentResult);
        }

        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssignmentId,StudentProfileId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            var exists = await _context.AssignmentResults.AnyAsync(a =>
                a.AssignmentId == assignmentResult.AssignmentId &&
                a.StudentProfileId == assignmentResult.StudentProfileId);

            if (exists)
            {
                ModelState.AddModelError("", "This student already has a result for this assignment.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
                ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", assignmentResult.StudentProfileId);
                return View(assignmentResult);
            }

            _context.Add(assignmentResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult == null) return NotFound();

            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentId,StudentProfileId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id) return NotFound();

            var exists = await _context.AssignmentResults.AnyAsync(a =>
                a.Id != assignmentResult.Id &&
                a.AssignmentId == assignmentResult.AssignmentId &&
                a.StudentProfileId == assignmentResult.StudentProfileId);

            if (exists)
            {
                ModelState.AddModelError("", "This student already has a result for this assignment.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
                ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", assignmentResult.StudentProfileId);
                return View(assignmentResult);
            }

            try
            {
                _context.Update(assignmentResult);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentResultExists(assignmentResult.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null) return NotFound();

            return View(assignmentResult);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult != null)
            {
                _context.AssignmentResults.Remove(assignmentResult);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}