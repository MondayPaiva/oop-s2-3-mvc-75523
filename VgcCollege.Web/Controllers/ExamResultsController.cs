using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Student")]
    public class ExamResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.ExamResults
                    .Include(e => e.Exam)
                    .Include(e => e.StudentProfile);

                return View(await applicationDbContext.ToListAsync());
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myResults = _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.StudentProfile)
                .Where(e =>
                    e.StudentProfile != null &&
                    e.StudentProfile.IdentityUserId == userId &&
                    e.Exam != null &&
                    e.Exam.ResultsReleased);

            return View(await myResults.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (examResult == null) return NotFound();

            if (User.IsInRole("Student"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (examResult.StudentProfile?.IdentityUserId != userId)
                {
                    return Forbid();
                }

                if (examResult.Exam == null || !examResult.Exam.ResultsReleased)
                {
                    return Forbid();
                }
            }

            return View(examResult);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ExamId,StudentProfileId,Score,Grade")] ExamResult examResult)
        {
            var exists = await _context.ExamResults.AnyAsync(e =>
                e.ExamId == examResult.ExamId &&
                e.StudentProfileId == examResult.StudentProfileId);

            if (exists)
            {
                ModelState.AddModelError("", "This student already has a result for this exam.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
                ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);
                return View(examResult);
            }

            _context.Add(examResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult == null) return NotFound();

            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);
            return View(examResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamId,StudentProfileId,Score,Grade")] ExamResult examResult)
        {
            if (id != examResult.Id) return NotFound();

            var exists = await _context.ExamResults.AnyAsync(e =>
                e.Id != examResult.Id &&
                e.ExamId == examResult.ExamId &&
                e.StudentProfileId == examResult.StudentProfileId);

            if (exists)
            {
                ModelState.AddModelError("", "This student already has a result for this exam.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
                ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);
                return View(examResult);
            }

            try
            {
                _context.Update(examResult);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamResultExists(examResult.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (examResult == null) return NotFound();

            return View(examResult);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult != null)
            {
                _context.ExamResults.Remove(examResult);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExamResultExists(int id)
        {
            return _context.ExamResults.Any(e => e.Id == id);
        }
    }
}