using FlixTv.Api.Application.Features.Comments.Commands.DeleteComment;
using FlixTv.Api.Application.Features.Comments.Queries.GetAllComments;
using FlixTv.Api.Application.Features.Comments.Queries.GetComment;
using FlixTv.Api.Application.Features.Reviews.Commands.DeleteReview;
using FlixTv.Api.Application.Features.Reviews.Queries.GetAllReviews;
using FlixTv.Api.Application.Features.Reviews.Queries.GetReview;
using FlixTv.Api.Application.Features.Reviews.Queries.GetReviewsCount;
using FlixTv.Api.Application.Utilities;
using FlixTv.Common.Models.RequestModels.Reviews;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReviewsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews(
        [FromQuery] int? authorId,
        [FromQuery] int? movieId,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromQuery] string? searchText,
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 1,
        [FromQuery] int pageSize = 10
        )
        {
            var request = new GetAllReviewsQueryRequest()
            {
                pageSize = pageSize,
                currentPage = currentPage,
            };

            if (authorId.HasValue)
                request.predicate = request.predicate.And(r => r.AuthorId == authorId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(r => r.MovieId == movieId.Value);

            if (minRating.HasValue)
                request.predicate = request.predicate.And(r => r.RatingPoint >= minRating.Value);

            if (maxRating.HasValue)
                request.predicate = request.predicate.And(r => r.RatingPoint <= maxRating.Value);

            if (!string.IsNullOrEmpty(searchText))
                request.predicate = request.predicate.And(r => r.Message.ToLower().Contains(searchText.ToLower()));

            if (!string.IsNullOrWhiteSpace(orderBy))
                switch (orderBy)
                {
                    case "createdDate":
                        request.orderBy = x => x.OrderByDescending(c => c.CreatedDate);
                        break;
                    case "rating":
                        request.orderBy = x => x.OrderByDescending(c => c.RatingPoint);
                        break;
                    default:
                        break;
                }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [HttpGet]
        [Route("{reviewId}")]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            var response = await mediator.Send(new GetReviewQueryRequest { ReviewId = reviewId });

            if (response is null)
                throw new Exception("Review was not found.");

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsCount(
        [FromQuery] int? authorId,
        [FromQuery] int? movieId,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromQuery] string? searchText
        )
        {
            var request = new GetReviewsCountQueryRequest();

            if (authorId.HasValue)
                request.predicate = request.predicate.And(r => r.AuthorId == authorId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(r => r.MovieId == movieId.Value);

            if (minRating.HasValue)
                request.predicate = request.predicate.And(r => r.RatingPoint >= minRating.Value);

            if (maxRating.HasValue)
                request.predicate = request.predicate.And(r => r.RatingPoint <= maxRating.Value);

            if (!string.IsNullOrEmpty(searchText))
                request.predicate = request.predicate.And(r => r.Message.ToLower().Contains(searchText.ToLower()));

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "Review was created succesfully." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "Review was updated succesfully." });
        }

        [HttpPost]
        [Route("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            await mediator.Send(new DeleteReviewCommandRequest { ReviewId = reviewId });

            return Ok(new { message = "Review was deleted successfully!" });
        }
    }
}
