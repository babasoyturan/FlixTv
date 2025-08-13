using FlixTv.Api.Application.Features.FavouriteMovies.Commands.DeleteFavouriteMovie;
using FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetAllFavouriteMovies;
using FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetFavouriteMovie;
using FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetFavouriteMoviesCount;
using FlixTv.Api.Application.Utilities;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.FavouriteMovies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FavouriteMoviesController : ControllerBase
    {
        private readonly IMediator mediator;

        public FavouriteMoviesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllFavouriteMovies(
        [FromQuery] int? userId,
        [FromQuery] int? movieId,
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetAllFavouriteMoviesQueryRequest()
            {
                pageSize = pageSize,
                currentPage = currentPage,
            };

            if (userId.HasValue)
                request.predicate = request.predicate.And(f => f.UserId == userId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(f => f.MovieId == movieId.Value);

            if (!string.IsNullOrWhiteSpace(orderBy))
                switch (orderBy)
                {
                    case "createdDate":
                        request.orderBy = x => x.OrderByDescending(v => v.CreatedDate);
                        break;
                    default:
                        break;
                }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMyFavouriteMovies(
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetAllFavouriteMoviesQueryRequest()
            {
                pageSize = pageSize,
                currentPage = currentPage,
            };

            if (!string.IsNullOrWhiteSpace(orderBy))
                switch (orderBy)
                {
                    case "createdDate":
                        request.orderBy = x => x.OrderByDescending(v => v.CreatedDate);
                        break;
                    default:
                        break;
                }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        [Route("{favouriteMovieId}")]
        public async Task<IActionResult> GetFavouriteMovie(int favouriteMovieId)
        {
            var response = await mediator.Send(new GetFavouriteMovieQueryRequest { FavouriteMovieId = favouriteMovieId });

            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetFavouriteMoviesCount(
        [FromQuery] int? userId,
        [FromQuery] int? movieId
        )
        {
            var request = new GetFavouriteMoviesCountQueryRequest();

            if (userId.HasValue)
                request.predicate = request.predicate.And(f => f.UserId == userId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(f => f.MovieId == movieId.Value);

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMyFavouriteMoviesCount()
        {
            var response = await mediator.Send(new GetFavouriteMoviesCountQueryRequest());

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> CreateFavouriteMovie([FromBody] CreateFavouriteMovieCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "The favourite movie was created successfully." });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        [Route("{favouriteMovieId}")]
        public async Task<IActionResult> DeleteFavouriteMovie(int favouriteMovieId)
        {
            await mediator.Send(new DeleteFavouriteMovieCommandRequest { FavouriteMovieId = favouriteMovieId });

            return Ok(new { message = "The favourite movie was deleted successfully!" });
        }
    }
}
