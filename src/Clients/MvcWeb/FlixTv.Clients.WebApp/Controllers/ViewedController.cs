using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class ViewedController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
