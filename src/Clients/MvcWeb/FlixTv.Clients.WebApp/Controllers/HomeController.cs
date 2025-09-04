using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Implementations;
using FlixTv.Clients.WebApp.ViewModels;
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
            var model = new HomeViewModel();

            var recommendedMoviesResponse = await moviesService.GetMoviesByUserCompatibilityAsync();

            if (recommendedMoviesResponse.IsSuccess)
                model.RecommendedMovies = recommendedMoviesResponse.Data;

            model.Rows = new List<RowViewModel>();

            var rowsResponse = await moviesService.GetRowModelsAsync(15);

            if (rowsResponse.IsSuccess)
            {
                foreach (var row in rowsResponse.Data)
                {
                    var rowModel = new RowViewModel
                    {
                        RowTitle = row.Title,
                        RowKey = row.RowKey
                    };

                    var rowMoviesResponse = await moviesService.GetMoviesForRowAsync(row, 12);

                    if (rowMoviesResponse.IsSuccess)
                        rowModel.Movies = rowMoviesResponse.Data;

                    model.Rows.Add(rowModel);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
