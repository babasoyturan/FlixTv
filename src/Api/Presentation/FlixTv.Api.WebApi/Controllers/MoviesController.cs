using FlixTv.Api.Application.Features.Movies.Commands.DeleteMovie;
using FlixTv.Api.Application.Features.Movies.Queries.GetAllMovies;
using FlixTv.Api.Application.Features.Movies.Queries.GetMovie;
using FlixTv.Api.Application.Features.Movies.Queries.GetMoviesByUserCompatibility;
using FlixTv.Api.Application.Features.Movies.Queries.GetMoviesCount;
using FlixTv.Api.Application.Features.Movies.Queries.GetRelatedMovies;
using FlixTv.Api.Application.Features.Movies.Queries.GetRowModels;
using FlixTv.Api.Application.Features.Movies.Queries.GetUnfinishedMovies;
using FlixTv.Api.Application.Utilities;
using FlixTv.Common.Models;
using FlixTv.Common.Models.RequestModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMediator mediator;

        public MoviesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies(
        [FromQuery] string? searchText,
        [FromQuery] int? maxReleaseYear,
        [FromQuery] int? minReleaseYear,
        [FromQuery] short? ageLimitation,
        [FromQuery] int? maxDuration,
        [FromQuery] int? minDuration,
        [FromQuery] byte? maxRating,
        [FromQuery] byte? minRating,
        [FromQuery] byte? maxViewCount,
        [FromQuery] byte? minViewCount,
        [FromQuery] bool? isVisible,
        [FromQuery] List<string> Categories,
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetAllMoviesQueryRequest()
            {
                currentPage = currentPage,
                pageSize = pageSize
            };

            if (!string.IsNullOrWhiteSpace(searchText))
                request.predicate = request.predicate.And(m => m.Title.Contains(searchText));

            if (minReleaseYear.HasValue)
                request.predicate = request.predicate.And(m => m.ReleaseYear >= minReleaseYear);

            if (maxReleaseYear.HasValue)
                request.predicate = request.predicate.And(m => m.ReleaseYear <= maxReleaseYear);

            if (ageLimitation.HasValue)
                request.predicate = request.predicate.And(m => m.AgeLimitation <= ageLimitation);

            if (minDuration.HasValue)
                request.predicate = request.predicate.And(m => m.Duration >= minDuration);

            if (maxDuration.HasValue)
                request.predicate = request.predicate.And(m => m.Duration <= maxDuration);

            if (minViewCount.HasValue)
                request.predicate = request.predicate.And(m => m.Views.Count() >= minViewCount);

            if (maxViewCount.HasValue)
                request.predicate = request.predicate.And(m => m.Views.Count() <= maxViewCount);

            if (minRating.HasValue)
                request.predicate = request.predicate.And(m => m.Rating >= minRating);

            if (maxRating.HasValue)
                request.predicate = request.predicate.And(m => m.Rating <= maxRating);

            if (isVisible.HasValue)
                request.predicate = request.predicate.And(m => m.IsVisible == isVisible);

            if (Categories != null && Categories.Any())
            {
                List<MovieCategory> categoryEnums = new();

                foreach (var cat in Categories)
                {
                    if (Enum.TryParse<MovieCategory>(cat, true, out var parsed))
                        categoryEnums.Add(parsed);
                    else
                        throw new Exception($"The {cat} category is not exist");
                }

                request.predicate = request.predicate.And(m =>
                    categoryEnums.All(c => m.Categories.Contains(c))
                );
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
                switch (orderBy)
                {
                    case "createdDate":
                        request.orderBy = x => x.OrderByDescending(m => m.CreatedDate);
                        break;
                    case "rating":
                        request.orderBy = x => x.OrderByDescending(m => m.Rating);
                        break;
                    case "releaseYear":
                        request.orderBy = x => x.OrderByDescending(m => m.ReleaseYear);
                        break;
                    case "viewCount":
                        request.orderBy = x => x.OrderByDescending(m => m.Views.Count());
                        break;
                    case "popular":
                        var twoMonthsAgo = DateTime.UtcNow.AddMonths(-2);

                        const double viewWeight = 1.0;
                        const double commentLikeWeight = 2.0;
                        const double commentDislikeWeight = 1.0;
                        const double reviewPosWeight = 8.0;
                        const double reviewNegWeight = 12.0;

                        request.orderBy = q => q.OrderByDescending(m =>
                            (double)m.Views.Count(v => v.CreatedDate >= twoMonthsAgo && v.IsCompleted)
                                * viewWeight

                            + (double)m.Comments
                                .Where(c => c.CreatedDate >= twoMonthsAgo)
                                .Sum(c => c.Likes.Count() * commentLikeWeight
                                       - c.Dislikes.Count() * commentDislikeWeight)

                            + m.Reviews
                                .Where(r => r.CreatedDate >= twoMonthsAgo && r.RatingPoint >= 6)
                                .Sum(r => ((double)r.RatingPoint / 10.0) * reviewPosWeight)

                            - m.Reviews
                                .Where(r => r.CreatedDate >= twoMonthsAgo && r.RatingPoint < 6)
                                .Sum(r => ((6.0 - (double)r.RatingPoint) / 6.0) * reviewNegWeight)

                            + m.Rating * 80
                        );
                        break;
                    default:
                        break;
                }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [HttpGet]
        [Route("{movieId}")]
        public async Task<IActionResult> GetMovie(int movieId)
        {
            var request = new GetMovieQueryRequest { MovieId = movieId };

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetMoviesCount(
        [FromQuery] string? searchText,
        [FromQuery] int? maxReleaseYear,
        [FromQuery] int? minReleaseYear,
        [FromQuery] short? ageLimitation,
        [FromQuery] int? maxDuration,
        [FromQuery] int? minDuration,
        [FromQuery] byte? maxRating,
        [FromQuery] byte? minRating,
        [FromQuery] byte? maxViewCount,
        [FromQuery] byte? minViewCount,
        [FromQuery] bool? isVisible,
        [FromQuery] List<string> Categories
        )
        {
            var request = new GetMoviesCountQueryRequest();

            if (!string.IsNullOrWhiteSpace(searchText))
                request.predicate = request.predicate.And(m => m.Title.Contains(searchText));

            if (minReleaseYear.HasValue)
                request.predicate = request.predicate.And(m => m.ReleaseYear >= minReleaseYear);

            if (maxReleaseYear.HasValue)
                request.predicate = request.predicate.And(m => m.ReleaseYear <= maxReleaseYear);

            if (ageLimitation.HasValue)
                request.predicate = request.predicate.And(m => m.AgeLimitation <= ageLimitation);

            if (minDuration.HasValue)
                request.predicate = request.predicate.And(m => m.Duration >= minDuration);

            if (maxDuration.HasValue)
                request.predicate = request.predicate.And(m => m.Duration <= maxDuration);

            if (minViewCount.HasValue)
                request.predicate = request.predicate.And(m => m.Views.Count() >= minViewCount);

            if (maxViewCount.HasValue)
                request.predicate = request.predicate.And(m => m.Views.Count() <= maxViewCount);

            if (minRating.HasValue)
                request.predicate = request.predicate.And(m => m.Rating >= minRating);

            if (maxRating.HasValue)
                request.predicate = request.predicate.And(m => m.Rating <= maxRating);

            if (isVisible.HasValue)
                request.predicate = request.predicate.And(m => m.IsVisible == isVisible);

            if (Categories != null && Categories.Any())
            {
                List<MovieCategory> categoryEnums = new();

                foreach (var cat in Categories)
                {
                    if (Enum.TryParse<MovieCategory>(cat, true, out var parsed))
                        categoryEnums.Add(parsed);
                    else
                        throw new Exception($"The {cat} category is not exist");
                }

                request.predicate = request.predicate.And(m =>
                    categoryEnums.All(c => m.Categories.Contains(c))
                );
            }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMoviesByUserCompatibility(
        [FromQuery] int count = 10
        )
        {
            var request = new GetMoviesByUserCompatibilityQueryRequest() { count = count };

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRelatedMovies([FromQuery] int movieId)
        {
            var request = new GetRelatedMoviesQueryRequest { MovieId = movieId };

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetUnfinishedMovies()
        {
            var response = await mediator.Send(new GetUnfinishedMoviesQueryRequest());

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRowModels([FromQuery] int count)
        {
            var request = new GetRowModelsQueryRequest { Count = count };
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "The Movie was created successfully" });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> UpdateMovie([FromForm] UpdateMovieCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "The Movie was updated successfully" });
        }

        
        [HttpPost]
        [Route("{movieId}")]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            var request = new DeleteMovieCommandRequest { MovieId = movieId };
            await mediator.Send(request);
            return Ok(new { message = "The Movie was deleted successfully" });
        }
    }
}