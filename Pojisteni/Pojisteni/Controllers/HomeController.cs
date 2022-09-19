using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pojisteni.Data;
using Pojisteni.Models;
using System.Data;
using System.Diagnostics;

namespace Pojisteni.Controllers
{
    [Authorize(Roles = "admin,insured")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if(User.IsInRole("insured"))
            {
                return RedirectToAction("Home", "Account");
            }
            ViewData["InsuredsCount"] = await _context.Insured.CountAsync();
            ViewData["InsuredInsurances"] = await _context.InsuranceInsured.CountAsync();
            ViewData["EventsCount"] = await _context.Event.CountAsync();
            return View();
        }
    }
}