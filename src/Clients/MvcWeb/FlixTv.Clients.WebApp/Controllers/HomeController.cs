using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Implementations;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.ResponseModels.Movies;
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

            var isAuth = User?.Identity?.IsAuthenticated ?? false;

            if (isAuth)
            {
                // “Recommended for you”
                var rec = await moviesService.GetMoviesByUserCompatibilityAsync(12);
                if (rec.IsSuccess && rec.Data != null)
                    model.RecommendedMovies = rec.Data;
                model.RecommendedTitlePart1 = "Recommended";
                model.RecommendedTitlePart2 = " for you";
            }
            else
            {
                // Qonaq: top rated pool 50 → random 12
                var poolRes = await moviesService.GetAllMoviesAsync(new MoviesFilter
                {
                    OrderBy = "rating",
                    Page = 1,
                    PageSize = 50
                });

                var pool = poolRes.IsSuccess ? (poolRes.Data ?? new List<GetAllMoviesQueryResponse>())
                                             : new List<GetAllMoviesQueryResponse>();

                model.RecommendedMovies = PickRandom(pool, 12).ToList();
                model.RecommendedTitlePart1 = "Best Movies";
                model.RecommendedTitlePart2 = " of FlixTV";
            }

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

        private static IList<T> PickRandom<T>(IList<T> source, int take)
        {
            if (source == null || source.Count == 0) return new List<T>();
            var list = source.ToList();
            var n = list.Count;
            take = Math.Min(Math.Max(1, take), n);

            var rng = Random.Shared;
            for (int i = 0; i < take; i++)
            {
                int j = rng.Next(i, n);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list.Take(take).ToList();
        }
    }
}
