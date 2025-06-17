using Microsoft.AspNetCore.Mvc;

namespace JuniorSteps.Controllers
{
    public class AccessController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == "baba123")
            {
                HttpContext.Session.SetString("admin_pass", "baba123");
                return Redirect("/secret-url");
            }

            ViewBag.Error = "Wrong password";
            return View();
        }
    }

}
