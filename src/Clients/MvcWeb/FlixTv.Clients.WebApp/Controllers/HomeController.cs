using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMoviesService moviesService;

        public HomeController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await moviesService.GetRowModelsAsync(10);

            foreach (var row in response.Data)
            {
                Console.WriteLine(row.Title);
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
