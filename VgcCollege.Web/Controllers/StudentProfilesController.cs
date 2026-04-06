using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Student")]
    public class StudentProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.StudentProfiles.ToListAsync());
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myProfile = await _context.StudentProfiles
                .Where(s => s.IdentityUserId == userId)
                .ToListAsync();

            return View(myProfile);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.Enrolments)
                    .ThenInclude(e => e.Course)
                .Include(s => s.AssignmentResults)
                    .ThenInclude(ar => ar.Assignment)
                .Include(s => s.ExamResults)
                    .ThenInclude(er => er.Exam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null) return NotFound();

            if (User.IsInRole("Student"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (studentProfile.IdentityUserId != userId)
                {
                    return Forbid();
                }
            }

            return View(studentProfile);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber")] StudentProfile studentProfile)
        {
            if (!ModelState.IsValid) return View(studentProfile);

            _context.Add(studentProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null) return NotFound();

            return View(studentProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber")] StudentProfile studentProfile)
        {
            if (id != studentProfile.Id) return NotFound();

            if (!ModelState.IsValid) return View(studentProfile);

            try
            {
                _context.Update(studentProfile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentProfileExists(studentProfile.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null) return NotFound();

            return View(studentProfile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }
    }
}