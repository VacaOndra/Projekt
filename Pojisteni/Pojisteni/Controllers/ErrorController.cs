using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pojisteni.Models;

namespace Pojisteni.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
            
            if(code == 401)
            {
                return View("~/Views/Error/401.cshtml");
            }
            else if (code == 500)
            {
                return View("~/Views/Error/500.cshtml");
            }
            else
            {
                return View("~/Views/Error/404.cshtml");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string text)
        {
            return View(new ErrorViewModel { Text = text });
        }
    }
}
