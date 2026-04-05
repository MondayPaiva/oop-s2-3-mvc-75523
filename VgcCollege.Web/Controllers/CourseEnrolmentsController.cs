using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseEnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var courseEnrolment = await _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile)
                .Include(c => c.AttendanceRecords)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (courseEnrolment == null) return NotFound();

            return View(courseEnrolment);
        }

        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentProfileId,CourseId,EnrolDate,Status")] CourseEnrolment courseEnrolment)
        {
            var exists = await _context.CourseEnrolments.AnyAsync(e =>
                e.StudentProfileId == courseEnrolment.StudentProfileId &&
                e.CourseId == courseEnrolment.CourseId);

            if (exists)
            {
                ModelState.AddModelError("", "This student is already enrolled in the selected course.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
                ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);
                return View(courseEnrolment);
            }

            _context.Add(courseEnrolment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var courseEnrolment = await _context.CourseEnrolments.FindAsync(id);
            if (courseEnrolment == null) return NotFound();

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);
            return View(courseEnrolment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentProfileId,CourseId,EnrolDate,Status")] CourseEnrolment courseEnrolment)
        {
            if (id != courseEnrolment.Id) return NotFound();

            var exists = await _context.CourseEnrolments.AnyAsync(e =>
                e.Id != courseEnrolment.Id &&
                e.StudentProfileId == courseEnrolment.StudentProfileId &&
                e.CourseId == courseEnrolment.CourseId);

            if (exists)
            {
                ModelState.AddModelError("", "This student is already enrolled in the selected course.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
                ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", courseEnrolment.StudentProfileId);
                return View(courseEnrolment);
            }

            try
            {
                _context.Update(courseEnrolment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseEnrolmentExists(courseEnrolment.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var courseEnrolment = await _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (courseEnrolment == null) return NotFound();

            return View(courseEnrolment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseEnrolment = await _context.CourseEnrolments.FindAsync(id);
            if (courseEnrolment != null)
            {
                _context.CourseEnrolments.Remove(courseEnrolment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseEnrolmentExists(int id)
        {
            return _context.CourseEnrolments.Any(e => e.Id == id);
        }
    }
}