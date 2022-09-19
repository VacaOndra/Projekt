using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pojisteni.Data;
using Pojisteni.Models;

namespace Pojisteni.Controllers
{
    [Authorize(Roles = "admin")]
    public class InsuranceInsuredsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsuranceInsuredsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InsuranceInsureds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.InsuranceInsured.Include(i => i.Insurance).Include(i => i.Insured);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: InsuranceInsureds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InsuranceInsured == null)
            {
                return NotFound();
            }

            var insuranceInsured = await _context.InsuranceInsured
                .Include(i => i.Insurance)
                .Include(i => i.Insured)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (insuranceInsured == null)
            {
                return NotFound();
            }

            return View(insuranceInsured);
        }

        // GET: InsuranceInsureds/Create
        public async Task<IActionResult> Create(int? insuredID)
        {
            if(insuredID == null)
            {
                return NotFound();
            }

            if(!await _context.Insurance.AnyAsync())
            {
                return RedirectToAction("Error", "Error", new { text = "Nemáte založené žádné pojištění" });
            }

            ViewData["InsuranceID"] = new SelectList(_context.Insurance, "ID", "Name");
            ViewData["Insured"] = await _context.Insured.FirstOrDefaultAsync(m => m.ID == insuredID);
            return View();
        }

        // POST: InsuranceInsureds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InsuredID,InsuranceID,Amount,Subject,StartsAt,EndsAt")] InsuranceInsured insuranceInsured, int insuredID)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insuranceInsured);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(InsuredsController.Details), "Insureds", new {id = insuranceInsured.InsuredID });
            }
            ViewData["InsuranceID"] = new SelectList(_context.Insurance, "ID", "Name", insuranceInsured.InsuranceID);
            ViewData["Insured"] = await _context.Insured.FirstOrDefaultAsync(m => m.ID == insuredID);
            return View(insuranceInsured);
        }

        // GET: InsuranceInsureds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InsuranceInsured == null)
            {
                return NotFound();
            }

            var insuranceInsured = await _context.InsuranceInsured
                .Include(i => i.Insurance)
                .Include(i => i.Insured)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (insuranceInsured == null)
            {
                return NotFound();
            }
            ViewData["InsuranceID"] = new SelectList(_context.Insurance, "ID", "Name", insuranceInsured.InsuranceID);
            ViewData["Insured"] = insuranceInsured.Insured;
            return View(insuranceInsured);
        }

        // POST: InsuranceInsureds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,InsuredID,InsuranceID,Amount,Subject,StartsAt,EndsAt")] InsuranceInsured insuranceInsured)
        {
            if (id != insuranceInsured.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceInsured);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceInsuredExists(insuranceInsured.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(InsuredsController.Details), "Insureds", new { id = insuranceInsured.InsuredID });
            }

            ViewData["InsuranceID"] = new SelectList(_context.Insurance, "ID", "Name", insuranceInsured.InsuranceID);
            ViewData["Insured"] = await _context.Insured.FirstOrDefaultAsync(m => m.ID == insuranceInsured.InsuredID);
            
            return View(insuranceInsured);
        }

        // GET: InsuranceInsureds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InsuranceInsured == null)
            {
                return NotFound();
            }

            var insuranceInsured = await _context.InsuranceInsured
                .Include(i => i.Insurance)
                .Include(i => i.Insured)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (insuranceInsured == null)
            {
                return NotFound();
            }

            return View(insuranceInsured);
        }

        // POST: InsuranceInsureds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InsuranceInsured == null)
            {
                return Problem("Entity set 'ApplicationDbContext.InsuranceInsured'  is null.");
            }
            var insuranceInsured = await _context.InsuranceInsured.FindAsync(id);
            if (insuranceInsured != null)
            {
                _context.InsuranceInsured.Remove(insuranceInsured);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InsuredsController.Details), "Insureds", new { id = insuranceInsured.InsuredID });
        }

        private bool InsuranceInsuredExists(int id)
        {
          return (_context.InsuranceInsured?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
