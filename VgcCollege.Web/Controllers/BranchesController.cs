using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Branches.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.Branches
                .Include(b => b.Courses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null) return NotFound();

            return View(branch);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] Branch branch)
        {
            if (!ModelState.IsValid) return View(branch);

            _context.Add(branch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return NotFound();

            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Branch branch)
        {
            if (id != branch.Id) return NotFound();

            if (!ModelState.IsValid) return View(branch);

            try
            {
                _context.Update(branch);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(branch.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.Branches.FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null) return NotFound();

            return View(branch);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}