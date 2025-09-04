using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public async Task<IActionResult> Watch(int id)
        {
            var movieRepsponse = await moviesService.GetMovieAsync(id);

            if (!movieRepsponse.IsSuccess)
                return RedirectToAction("Index", "Home");

            var similiarMoviesResponse = await moviesService.GetRelatedMoviesAsync(id);

            if (!similiarMoviesResponse.IsSuccess)
                return RedirectToAction("Index", "Home");

            var model = new WatchViewModel()
            {
                Movie = movieRepsponse.Data,
                SimiliarMovies = similiarMoviesResponse.Data
            };

            return View(model);
        }
    }
}
