using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    public class InsuredsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AccountIdentity> _userManager;

        public InsuredsController(ApplicationDbContext context, UserManager<AccountIdentity> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "admin")]
        // GET: Insureds
        public async Task<IActionResult> Index()
        {
            return _context.Insured != null ?
                        View(await _context.Insured.OrderBy(m => m.LastName).Include(m => m.InsuranceInsureds).ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Insured'  is null.");
        }

        [Authorize(Roles = "admin,insured")]
        // GET: Insureds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(User.IsInRole("insured"))
            {
                return RedirectToAction("Home", "Account");
            }
            if (id == null || _context.Insured == null)
            {
                return NotFound();
            }

            var insured = _context.Insured
                .Where(m => m.ID == id)
                .Include(m => m.InsuranceInsureds)
                .ThenInclude(m => m.Insurance)
                .FirstOrDefault();

            ViewData["InsuranceCount"] = await _context.Insurance.CountAsync();
            ViewData["HasLogin"] = await _context.Users.AnyAsync(m => m.InsuredID == insured.ID);

            if (insured == null)
            {
                return NotFound();
            }

            return View(insured);
        }

        [Authorize(Roles = "admin")]
        // GET: Insureds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Insureds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LastName,Email,PhoneNumber,IdentityNumber,Adress,City,PostalCode")] Insured insured)
        {
            if (ModelState.IsValid)
            {
                if(!await _context.Insured.AnyAsync(m => m.Email == insured.Email))
                {
                    _context.Add(insured);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tento email již v databázi existuje.");
                }
            }
            return View(insured);
        }

        [Authorize(Roles = "admin")]
        // GET: Insureds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Insured == null)
            {
                return NotFound();
            }

            var insured = await _context.Insured.FindAsync(id);
            if (insured == null)
            {
                return NotFound();
            }
            return View(insured);
        }

        // POST: Insureds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,LastName,Email,PhoneNumber,IdentityNumber,Adress,City,PostalCode")] Insured insured)
        {
            if (id != insured.ID)
            {
                return NotFound();
            }
            if (await _context.Insured.AnyAsync(m => m.Email == insured.Email && m.ID != insured.ID))
            {
                ModelState.AddModelError(string.Empty, "Tento email již v databázi existuje.");
                return View(insured);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                        _context.Update(insured);
                        var user = await _context.Users.FirstOrDefaultAsync(m => m.InsuredID == insured.ID);
                        if (user != null)
                        {
                            await _userManager.SetEmailAsync(user, insured.Email);
                            await _userManager.SetUserNameAsync(user, insured.Email);
                        }
                        await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuredExists(insured.ID))
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
            return View(insured);
        }

        [Authorize(Roles = "admin")]
        // GET: Insureds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Insured == null)
            {
                return NotFound();
            }

            var insured = await _context.Insured
                .FirstOrDefaultAsync(m => m.ID == id);
            if (insured == null)
            {
                return NotFound();
            }

            return View(insured);
        }

        [Authorize(Roles = "admin")]
        // POST: Insureds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Insured == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Insured'  is null.");
            }
            var insured = await _context.Insured.FindAsync(id);
            
            if (insured != null)
            {
                _context.Insured.Remove(insured);
                var user = await _userManager.FindByEmailAsync(insured.Email);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuredExists(int id)
        {
            return (_context.Insured?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
