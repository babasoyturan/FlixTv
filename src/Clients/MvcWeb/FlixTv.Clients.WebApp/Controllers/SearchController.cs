using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.ResponseModels.Movies;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IMoviesService moviesService;
        public SearchController(IMoviesService moviesService) => this.moviesService = moviesService;

        // Tam səhifə
        [HttpGet]
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 54)
        {
            var text = string.IsNullOrWhiteSpace(q) ? null : q.Trim();

            var filter = new MoviesFilter
            {
                SearchText = text,
                // heç bir filter/order göstərmirik
                OrderBy = null,
                Page = page,
                PageSize = pageSize
            };

            var listRes = await moviesService.GetAllMoviesAsync(filter);
            var countRes = await moviesService.GetMoviesCountAsync(filter);

            var vm = new MoviesListPartialViewModel
            {
                Movies = listRes.IsSuccess ? (listRes.Data ?? new List<GetAllMoviesQueryResponse>()) : new List<GetAllMoviesQueryResponse>(),
                TotalCount = countRes.IsSuccess ? countRes.Data : 0,
                Page = page,
                PageSize = pageSize,
                Query = new() { ["q"] = text }, // yalnız axtarış mətni
                ListController = "Search",
                ListAction = "SearchResults"
            };

            ViewBag.QueryText = text ?? "";
            return View(vm);
        }

        // AJAX partial (paginator üçün)
        [HttpGet]
        public async Task<IActionResult> SearchResults(string? q, int page = 1, int pageSize = 24)
        {
            var text = string.IsNullOrWhiteSpace(q) ? null : q.Trim();

            var filter = new MoviesFilter
            {
                SearchText = text,
                OrderBy = null,
                Page = page,
                PageSize = pageSize
            };

            var listRes = await moviesService.GetAllMoviesAsync(filter);
            var countRes = await moviesService.GetMoviesCountAsync(filter);
            if (!listRes.IsSuccess || !countRes.IsSuccess)
                return StatusCode(500, "Could not load search results.");

            var vm = new MoviesListPartialViewModel
            {
                Movies = listRes.Data ?? new List<GetAllMoviesQueryResponse>(),
                TotalCount = countRes.Data,
                Page = page,
                PageSize = pageSize,
                Query = new() { ["q"] = text },
                ListController = "Search",
                ListAction = "SearchResults"
            };

            return PartialView("_MoviesGrid", vm);
        }
    }
}
