using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Implementations;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.ResponseModels.Comments;
using FlixTv.Common.Models.ResponseModels.Movies;
using FlixTv.Common.Models.ResponseModels.Reviews;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMoviesService moviesService;
        private readonly ICommentsService commentsService;
        private readonly IReviewsService reviewsService;

        public MoviesController(IMoviesService moviesService, ICommentsService commentsService, IReviewsService reviewsService)
        {
            this.moviesService = moviesService;
            this.commentsService = commentsService;
            this.reviewsService = reviewsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
        string? searchText, List<string>? categories, string? orderBy,
        int? minReleaseYear, int? maxReleaseYear, int page = 1, int pageSize = 54)
        {
            var filter = new MoviesFilter
            {
                SearchText = searchText,
                Categories = categories ?? new(),
                OrderBy = string.IsNullOrWhiteSpace(orderBy) ? "rating" : orderBy,
                MinReleaseYear = minReleaseYear,
                MaxReleaseYear = maxReleaseYear,
                Page = page,
                PageSize = pageSize
            };

            var model = new MoviesViewModel { Filter = filter };

            var listRes = await moviesService.GetAllMoviesAsync(filter);
            var countRes = await moviesService.GetMoviesCountAsync(filter);

            if (listRes.IsSuccess) model.List.Movies = listRes.Data ?? new List<GetAllMoviesQueryResponse>();
            if (countRes.IsSuccess) model.List.TotalCount = countRes.Data;

            model.List.Page = filter.Page;
            model.List.PageSize = filter.PageSize;
            model.List.Query = BuildQueryDict(filter);

            return View(model);
        }

        public async Task<IActionResult> Watch(int id)
        {
            var model = new WatchViewModel()
            {
                CommentsPage = 1,
                CommentsPageSize = 5,
                ReviewsPage = 1,
                ReviewsPageSize = 5
            };

            var movieRepsponse = await moviesService.GetMovieAsync(id);

            if (!movieRepsponse.IsSuccess)
                return RedirectToAction("Index", "Home");

            model.Movie = movieRepsponse.Data;

            var similiarMoviesResponse = await moviesService.GetRelatedMoviesAsync(id);

            if (similiarMoviesResponse.IsSuccess)
                model.SimiliarMovies = similiarMoviesResponse.Data;


            var latestPoolRes = await moviesService.GetLatestMoviesAsync(pool: 50);
            if (latestPoolRes.IsSuccess && latestPoolRes.Data != null && latestPoolRes.Data.Count > 0)
            {
                model.NewItems = PickRandom(latestPoolRes.Data, take: 6);
            }

            var commentsCountResponse = await commentsService.GetMovieCommentsCountAsync(id);
            if (commentsCountResponse.IsSuccess)
                model.CommentsCount = commentsCountResponse.Data;

            if (model.CommentsCount > 0)
            {
                var commentsResponse = await commentsService.GetMovieCommentsAsync(id, model.CommentsPage, model.CommentsPageSize);
                if (commentsResponse.IsSuccess) model.Comments = commentsResponse.Data ?? new List<GetCommentQueryResponse>();
            }


            var reviewsCountResponse = await reviewsService.GetMovieReviewsCountAsync(id);
            if (reviewsCountResponse.IsSuccess) model.ReviewsCount = reviewsCountResponse.Data;

            if (model.ReviewsCount > 0)
            {
                var reviewsResponse = await reviewsService.GetMovieReviewsAsync(id, model.ReviewsPage, model.ReviewsPageSize, "createdDate");
                if (reviewsResponse.IsSuccess) model.Reviews = reviewsResponse.Data ?? new List<GetReviewQueryResponse>();
            }

            model.CanAddReview = false;
            if (User?.Identity?.IsAuthenticated == true)
            {
                var uidStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(uidStr, out var uid))
                {
                    var myCountRes = await reviewsService.GetMovieUserReviewCountAsync(id, uid);
                    model.CanAddReview = myCountRes.IsSuccess && myCountRes.Data == 0;
                }
            }


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ContinueWatching(int page = 1, int pageSize = 12)
        {
            // login yoxdursa – Signin-ə yönləndir
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return RedirectToAction("Signin", "Account",
                    new { returnUrl = Url.Action("ContinueWatching", "Movies") });

            var res = await moviesService.GetUnfinishedMoviesAsync();
            var all = res.IsSuccess ? (res.Data ?? new List<GetAllMoviesQueryResponse>())
                                    : new List<GetAllMoviesQueryResponse>();

            var vm = new MoviesViewModel
            {
                List = new MoviesListPartialViewModel
                {
                    Movies = all.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                    TotalCount = all.Count,
                    Page = page,
                    PageSize = pageSize,
                    Query = new Dictionary<string, string?>(),
                    ListController = "Movies",
                    ListAction = "ContinueGrid"
                }
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ContinueGrid(int page = 1, int pageSize = 12)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            var res = await moviesService.GetUnfinishedMoviesAsync();
            if (!res.IsSuccess) return StatusCode(500, "Could not load movies.");

            var all = res.Data ?? new List<GetAllMoviesQueryResponse>();

            var vm = new MoviesListPartialViewModel
            {
                Movies = all.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = all.Count,
                Page = page,
                PageSize = pageSize,
                Query = new Dictionary<string, string?>(),
                ListController = "Movies",
                ListAction = "ContinueGrid"
            };

            return PartialView("_MoviesGrid", vm);
        }

        [HttpGet]
        public async Task<IActionResult> MoviesGrid(
        string? searchText, List<string>? categories, string? orderBy,
        int? minReleaseYear, int? maxReleaseYear, int page = 1, int pageSize = 12)
        {
            var filter = new MoviesFilter
            {
                SearchText = searchText,
                Categories = categories ?? new(),
                OrderBy = string.IsNullOrWhiteSpace(orderBy) ? "rating" : orderBy,
                MinReleaseYear = minReleaseYear,
                MaxReleaseYear = maxReleaseYear,
                Page = page,
                PageSize = pageSize
            };

            var listRes = await moviesService.GetAllMoviesAsync(filter);
            var countRes = await moviesService.GetMoviesCountAsync(filter);
            if (!listRes.IsSuccess || !countRes.IsSuccess)
                return StatusCode(500, "Could not load movies.");

            var vm = new MoviesListPartialViewModel
            {
                Movies = listRes.Data ?? new List<GetAllMoviesQueryResponse>(),
                TotalCount = countRes.Data,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Query = BuildQueryDict(filter)
            };

            return PartialView("_MoviesGrid", vm);
        }

        [HttpGet]
        public async Task<IActionResult> MovieComments(int movieId, int page = 1, int pageSize = 10)
        {
            var countRes = await commentsService.GetMovieCommentsCountAsync(movieId);
            var listRes = await commentsService.GetMovieCommentsAsync(movieId, page, pageSize, "createdDate");

            if (!listRes.IsSuccess || !countRes.IsSuccess)
                return StatusCode(500, "Could not load comments.");

            var vm = new MovieCommentsPartialViewModel
            {
                MovieId = movieId,
                TotalCount = countRes.Data,
                Comments = listRes.Data ?? new List<GetCommentQueryResponse>(),
                Page = page,
                PageSize = pageSize
            };

            return PartialView("_MovieComments", vm);
        }

        [HttpGet]
        public async Task<IActionResult> MovieReviews(int movieId, int page = 1, int pageSize = 5)
        {
            var countRes = await reviewsService.GetMovieReviewsCountAsync(movieId);
            var listRes = await reviewsService.GetMovieReviewsAsync(movieId, page, pageSize, "createdDate");

            if (!listRes.IsSuccess || !countRes.IsSuccess)
                return StatusCode(500, "Could not load reviews.");

            var vm = new MovieReviewsPartialViewModel
            {
                MovieId = movieId,
                TotalCount = countRes.Data,
                Reviews = listRes.Data ?? new List<GetReviewQueryResponse>(),
                Page = page,
                PageSize = pageSize
            };

            return PartialView("_MovieReviews", vm);
        }

        private static Dictionary<string, string?> BuildQueryDict(MoviesFilter f) => new()
        {
            ["searchText"] = f.SearchText,
            ["orderBy"] = f.OrderBy,
            ["minReleaseYear"] = f.MinReleaseYear?.ToString(),
            ["maxReleaseYear"] = f.MaxReleaseYear?.ToString(),
            // Categories çoxlu gəlir – paging linklərində ayrıca əlavə edəcəyik
            // Page və PageSize linklərdə ayrıca yazılır
        };

        private static IList<T> PickRandom<T>(IList<T> source, int take)
        {
            if (source == null || source.Count == 0) return new List<T>();
            var list = source.ToList();
            var n = list.Count;
            take = Math.Min(Math.Max(1, take), n);

            var rng = Random.Shared; // .NET 6+
            for (int i = 0; i < take; i++)
            {
                int j = rng.Next(i, n); // [i, n)
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list.Take(take).ToList();
        }
    }
}
