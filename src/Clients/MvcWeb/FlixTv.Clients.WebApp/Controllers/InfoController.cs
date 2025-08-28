using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class InfoController : Controller
    {
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            if (!TempData.ContainsKey("Status") || !TempData.ContainsKey("Message"))
                return RedirectToAction("Index", "Home");

            Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

            return View();
        }
    }
}
