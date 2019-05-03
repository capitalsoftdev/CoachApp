using System.Linq;
using System.Threading.Tasks;
using CoachApp.Data;
using CoachApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachApp.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext _context;
        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tests = _context.Tests
                .AsNoTracking()
                .Include(t => t.Participants)
                .OrderByDescending(t => t.Date)
                .ToList();
            return View(tests);
        }

        [HttpGet]
        [Authorize(Roles = "Coach")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Create(
            [Bind("Type,Date")] Test test)
        {
            if (ModelState.IsValid)
            {
                _context.Tests.Add(test);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Details(string id)
        {
            var test = _context.Tests
                .AsNoTracking()
                .Include(t => t.Participants)
                    .ThenInclude(p => p.User)
                .FirstOrDefault(t=>t.Id == id);
            return View(test);
        }

        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (test == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(test);
        }

        [Authorize(Roles = "Coach")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var test = await _context.Tests
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Tests.Remove(test);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}