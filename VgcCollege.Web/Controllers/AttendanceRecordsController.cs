using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AttendanceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile!)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course!);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile!)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course!)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attendanceRecord == null) return NotFound();

            return View(attendanceRecord);
        }

        public IActionResult Create()
        {
            ViewData["CourseEnrolmentId"] = new SelectList(
                _context.CourseEnrolments
                    .Include(e => e.StudentProfile)
                    .Include(e => e.Course)
                    .ToList()
                    .Select(e => new
                    {
                        e.Id,
                        Display = $"{e.StudentProfile?.Name ?? "-"} - {e.Course?.Name ?? "-"}"
                    }),
                "Id",
                "Display");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseEnrolmentId,SessionDate,Present")] AttendanceRecord attendanceRecord)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CourseEnrolmentId"] = new SelectList(
                    _context.CourseEnrolments
                        .Include(e => e.StudentProfile)
                        .Include(e => e.Course)
                        .ToList()
                        .Select(e => new
                        {
                            e.Id,
                            Display = $"{e.StudentProfile?.Name ?? "-"} - {e.Course?.Name ?? "-"}"
                        }),
                    "Id",
                    "Display",
                    attendanceRecord.CourseEnrolmentId);

                return View(attendanceRecord);
            }

            _context.Add(attendanceRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord == null) return NotFound();

            ViewData["CourseEnrolmentId"] = new SelectList(
                _context.CourseEnrolments
                    .Include(e => e.StudentProfile)
                    .Include(e => e.Course)
                    .ToList()
                    .Select(e => new
                    {
                        e.Id,
                        Display = $"{e.StudentProfile?.Name ?? "-"} - {e.Course?.Name ?? "-"}"
                    }),
                "Id",
                "Display",
                attendanceRecord.CourseEnrolmentId);

            return View(attendanceRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseEnrolmentId,SessionDate,Present")] AttendanceRecord attendanceRecord)
        {
            if (id != attendanceRecord.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CourseEnrolmentId"] = new SelectList(
                    _context.CourseEnrolments
                        .Include(e => e.StudentProfile)
                        .Include(e => e.Course)
                        .ToList()
                        .Select(e => new
                        {
                            e.Id,
                            Display = $"{e.StudentProfile?.Name ?? "-"} - {e.Course?.Name ?? "-"}"
                        }),
                    "Id",
                    "Display",
                    attendanceRecord.CourseEnrolmentId);

                return View(attendanceRecord);
            }

            try
            {
                _context.Update(attendanceRecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceRecordExists(attendanceRecord.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile!)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course!)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attendanceRecord == null) return NotFound();

            return View(attendanceRecord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord != null)
            {
                _context.AttendanceRecords.Remove(attendanceRecord);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceRecordExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.Id == id);
        }
    }
}