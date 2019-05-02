using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoachApp.Data;
using CoachApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoachApp.Controllers
{
    [Authorize(Roles = "Coach")]
    public class ParticipantController : Controller
    {
        private ApplicationDbContext _context;
        public ParticipantController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Create(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = _context.Tests.FirstOrDefault(t => t.Id == id);
            if(test == null)
            {
                return NotFound();
            }
            
            ViewBag.Test = test;
            ViewBag.Athlets = GetAthlets();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> Create(
            [Bind("TestId,UserId,DistanceResult,TimeResult")] Participant participant)
        {
            var test = _context.Tests
                .Include(t => t.Participants)
                .FirstOrDefault(t => t.Id == participant.TestId);
            if (ModelState.IsValid)
            {
                if (test != null)
                {
                    test.Participants.Add(participant);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(TestController.Details), "Test", new { id = test.Id });
                }
            }
            ViewBag.Test = test;
            ViewBag.Athlets = GetAthlets();
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Athlets = GetAthlets();
            var participant = await _context.Set<Participant>().Include(p => p.Test).FirstOrDefaultAsync(p => p.Id == id);
            if (participant == null)
            {
                return NotFound();
            }
            return View(participant);
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Athlets = GetAthlets();
            var participantToUpdate = await _context.Set<Participant>().Include(p => p.Test).FirstOrDefaultAsync(p => p.Id == id);
            if (await TryUpdateModelAsync<Participant>(
                participantToUpdate,
                "",
                p => p.UserId, p => p.DistanceResult, p => p.TimeResult))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Test", new { id = participantToUpdate.TestId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(participantToUpdate);
        }

        public async Task<IActionResult> Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Set<Participant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (participant == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(participant);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var participant = await _context.Set<Participant>()
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == id);
            if (participant == null)
            {
                return RedirectToAction(nameof(TestController.Details), "Test", new { id = participant.TestId});
            }

            try
            {
                _context.Set<Participant>().Remove(participant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TestController.Details), "Test", new { id = participant.TestId });
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private IQueryable<SelectListItem> GetAthlets()
        {
            return (from r in _context.Roles
                    join ur in _context.UserRoles on r.Id equals ur.RoleId
                    join u in _context.Users on ur.UserId equals u.Id
                    where (r.Name == "Athlete")
                    select new SelectListItem(u.FullName, u.Id));
        }
    }
}