using Microsoft.AspNetCore.Mvc;

namespace YouthParliamentApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Redirect($"/Identity/Account/Login?returnUrl={returnUrl}");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Redirect("/Identity/Account/AccessDenied");
        }
    }
}
