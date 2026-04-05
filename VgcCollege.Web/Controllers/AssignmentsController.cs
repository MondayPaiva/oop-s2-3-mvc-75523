using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Assignments
                .Include(a => a.Course);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Results)
                    .ThenInclude(r => r.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,Title,MaxScore,DueDate")] Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
                return View(assignment);
            }

            _context.Add(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,Title,MaxScore,DueDate")] Assignment assignment)
        {
            if (id != assignment.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
                return View(assignment);
            }

            try
            {
                _context.Update(assignment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(assignment.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}