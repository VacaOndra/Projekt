using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pojisteni.Data;
using Pojisteni.Models;

namespace Pojisteni.Controllers
{
    [Authorize(Roles = "admin,insured")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AccountIdentity> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<AccountIdentity> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            if(User.IsInRole("admin"))
            {
                var applicationDbContext = _context.Event.Include(i => i.InsuranceInsured).ThenInclude(i => i.Insurance).Include(i => i.InsuranceInsured).ThenInclude(i => i.Insured);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var applicationDbContext = _context.Event.Where(m => m.InsuranceInsured.Insured.ID == user.InsuredID).Include(i => i.InsuranceInsured).ThenInclude(i => i.Insurance).Include(i => i.InsuranceInsured).ThenInclude(i => i.Insured);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Event == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            var @event = await _context.Event
                .Include(m => m.InsuranceInsured)
                .ThenInclude(m => m.Insurance)
                .ThenInclude(m => m.InsuranceInsureds)
                .ThenInclude(m => m.Insured)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (@event == null)
            {
                return NotFound();
            }


            if(!User.IsInRole("admin") && @event.InsuranceInsured.Insured.ID != user.InsuredID)
            {
                return RedirectToAction("Error" , "Error" , new { code = 401 });
            }

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create(int? insuranceInsuredID)
        {
            if (insuranceInsuredID == null)
            {
                return NotFound();
            }

            if (!await _context.InsuranceInsured.AnyAsync(m => m.ID == insuranceInsuredID))
            {
                return NotFound();
            }

            var InsuranceInsured = await _context.InsuranceInsured.FirstOrDefaultAsync(m => m.ID == insuranceInsuredID);

            if (InsuranceInsured.Status)
            {
                return RedirectToAction("Error", "Error", new { text = "K neaktivní pojistce nelze vložit pojistnou událost !" });
            }

            ViewData["InsuranceInsured"] = InsuranceInsured;
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,InsuranceInsuredID,Description, DateTime, InsertDateTime, Settled")] Event @event, int insuranceInsuredID)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                var insured = await _context.InsuranceInsured.Where(m => m.ID == insuranceInsuredID).Include(m => m.Insured).Select(m => m.Insured.ID).FirstOrDefaultAsync();
                return RedirectToAction("Details" , "Insureds" , new { id = insured });
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Event == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,InsuranceInsuredID,Description, DateTime, InsertDateTime, Settled")] Event @event)
        {
            if (id != @event.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Event == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(m => m.InsuranceInsured)
                .ThenInclude(m => m.Insurance)
                .ThenInclude(m => m.InsuranceInsureds)
                .ThenInclude(m => m.Insured)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Event == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Event'  is null.");
            }
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
          return (_context.Event?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetDone(int id)
        {
            if (id == null || _context.Event == null)
            {
                return NotFound();
            }
            if (!EventExists(id))
            {
                return RedirectToAction("Error", "Error", new { text = "Pojistná událost neexistuje !" });
            }

            var @event = await _context.Event.FindAsync(id);
            @event.Settled = true;
            _context.Update(@event);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
