using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.Courses
                    .Include(c => c.Branch)
                    .Include(c => c.FacultyProfile);

                return View(await applicationDbContext.ToListAsync());
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var facultyCourses = _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.FacultyProfile)
                .Where(c => c.FacultyProfile != null && c.FacultyProfile.IdentityUserId == userId);

            return View(await facultyCourses.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.FacultyProfile)
                .Include(c => c.Enrolments)
                    .ThenInclude(e => e.StudentProfile)
                .Include(c => c.Assignments)
                .Include(c => c.Exams)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (course.FacultyProfile?.IdentityUserId != userId)
                {
                    return Forbid();
                }
            }

            return View(course);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name");
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,BranchId,FacultyProfileId,StartDate,EndDate")] Course course)
        {
            if (!ModelState.IsValid)
            {
                ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
                ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Name", course.FacultyProfileId);
                return View(course);
            }

            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Name", course.FacultyProfileId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BranchId,FacultyProfileId,StartDate,EndDate")] Course course)
        {
            if (id != course.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
                ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Name", course.FacultyProfileId);
                return View(course);
            }

            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.FacultyProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}