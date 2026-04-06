using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty,Student")]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.AssignmentResults
                    .Include(a => a.Assignment)
                        .ThenInclude(a => a.Course)
                    .Include(a => a.StudentProfile);

                return View(await applicationDbContext.ToListAsync());
            }

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var facultyResults = _context.AssignmentResults
                    .Include(a => a.Assignment)
                        .ThenInclude(a => a.Course)
                            .ThenInclude(c => c.FacultyProfile)
                    .Include(a => a.StudentProfile)
                    .Where(a => a.Assignment != null &&
                                a.Assignment.Course != null &&
                                a.Assignment.Course.FacultyProfile != null &&
                                a.Assignment.Course.FacultyProfile.IdentityUserId == userId);

                return View(await facultyResults.ToListAsync());
            }

            var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myResults = _context.AssignmentResults
                .Include(a => a.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(a => a.StudentProfile)
                .Where(a => a.StudentProfile != null && a.StudentProfile.IdentityUserId == studentUserId);

            return View(await myResults.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assignment)
                    .ThenInclude(a => a.Course)
                        .ThenInclude(c => c.FacultyProfile)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null) return NotFound();

            if (User.IsInRole("Student"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (assignmentResult.StudentProfile?.IdentityUserId != userId)
                {
                    return Forbid();
                }
            }

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (assignmentResult.Assignment?.Course?.FacultyProfile?.IdentityUserId != userId)
                {
                    return Forbid();
                }
            }

            return View(assignmentResult);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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