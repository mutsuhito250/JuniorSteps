using Microsoft.AspNetCore.Mvc;

namespace JuniorSteps.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == "Maqa_2025M") 
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Wrong password";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            return View();
        }
    }

}
