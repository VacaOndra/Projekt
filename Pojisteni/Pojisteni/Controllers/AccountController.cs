using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pojisteni.Data;
using Pojisteni.Models;
using System.Data;

namespace Pojisteni.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AccountIdentity> userManager;
        private readonly SignInManager<AccountIdentity> signInManager;
        private readonly ApplicationDbContext _context;


        public AccountController
        (
            UserManager<AccountIdentity> userManager,
            SignInManager<AccountIdentity> signInManager,
            ApplicationDbContext context
        )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._context = context;
        }
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Neplatné heslo.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Neplatný email.");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> RegisterInsured(int? insuredID, string? returnUrl = null)
        {
            if (insuredID == null)
            {
                return NotFound();
            }

            if (!await _context.Insured.AnyAsync(m => m.ID == insuredID))
            {
                return RedirectToAction("Error", "Error", new { text = "Pojištěnec s tímto ID neexistuje" });
            }
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Insured"] = await _context.Insured.FirstOrDefaultAsync(m => m.ID == insuredID);
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterInsured(RegisterViewModel model, int? insuredID, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new AccountIdentity { UserName = model.Email, Email = model.Email, InsuredID = insuredID };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "insured");
                    return RedirectToAction("Details", "Insureds" , new { id = insuredID });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            if(insuredID == null)
            {
                return RedirectToAction("Error", "Home", new { text = "Chyba" });
            }
            ViewData["Insured"] = await _context.Insured.FirstOrDefaultAsync(m => m.ID == insuredID);
            return View(model);
        }

        [Authorize(Roles = "admin,insured")]
        public async Task<IActionResult> Home()
        {
            var user = await userManager.GetUserAsync(User);
            Insured insured = await _context.Insured
                .Where(m => m.ID == user.InsuredID)
                .Include(m => m.InsuranceInsureds)
                .ThenInclude(m => m.Insurance)
                .FirstOrDefaultAsync();

            if (insured == null)
            {
                return RedirectToAction("Error", "Error", new { text = "Neplatný uživatel" });
            }

            return View(insured);
        }


        [Authorize(Roles = "admin,insured")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize(Roles = "admin,insured")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                var result = await userManager.ChangePasswordAsync(user,
                    model.OldPassword, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }
    }
}
