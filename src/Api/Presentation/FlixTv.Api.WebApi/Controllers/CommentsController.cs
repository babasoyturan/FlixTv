using Azure.Core;
using FlixTv.Api.Application.Features.Comments.Commands.DecrementCommentDislike;
using FlixTv.Api.Application.Features.Comments.Commands.DecrementCommentLike;
using FlixTv.Api.Application.Features.Comments.Commands.DeleteComment;
using FlixTv.Api.Application.Features.Comments.Commands.IncrementCommentDislike;
using FlixTv.Api.Application.Features.Comments.Commands.IncrementCommentLike;
using FlixTv.Api.Application.Features.Comments.Queries.GetAllComments;
using FlixTv.Api.Application.Features.Comments.Queries.GetComment;
using FlixTv.Api.Application.Features.Comments.Queries.GetCommentsCount;
using FlixTv.Api.Application.Features.Comments.Queries.GetMyComments;
using FlixTv.Api.Application.Utilities;
using FlixTv.Common.Models.RequestModels.Comments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator mediator;

        public CommentsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments(
        [FromQuery] int? authorId,
        [FromQuery] int? movieId,
        [FromQuery] string? searchText,
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetAllCommentsQueryRequest() { 
                pageSize = pageSize,
                currentPage = currentPage,
            };

            if (authorId.HasValue)
                request.predicate = request.predicate.And(c => c.AuthorId == authorId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(c => c.MovieId == movieId.Value);

            if (!string.IsNullOrEmpty(searchText))
                request.predicate = request.predicate.And(c => c.Message.ToLower().Contains(searchText.ToLower()));

            if (!string.IsNullOrWhiteSpace(orderBy))
                switch (orderBy)
                {
                    case "createdDate":
                        request.orderBy = x => x.OrderByDescending(c => c.CreatedDate); 
                        break;
                    case "likeCount":
                        request.orderBy = x => x.OrderByDescending(c => c.Likes.Count());
                        break;
                    case "dislikeCount":
                        request.orderBy = x => x.OrderByDescending(c => c.Dislikes.Count());
                        break;
                    default:
                        break;
                }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMyComments(
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetMyCommentsQueryRequest()
            {
                pageSize = pageSize,
                currentPage = currentPage,
            };

            if (!string.IsNullOrWhiteSpace(orderBy))
                switch (orderBy)
                {
                    case "createdDate":
                        request.orderBy = x => x.OrderByDescending(c => c.CreatedDate);
                        break;
                    case "likeCount":
                        request.orderBy = x => x.OrderByDescending(c => c.Likes.Count());
                        break;
                    case "dislikeCount":
                        request.orderBy = x => x.OrderByDescending(c => c.Dislikes.Count());
                        break;
                    default:
                        break;
                }

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [HttpGet]
        [Route("{commentId}")]
        public async Task<IActionResult> GetComment(int commentId)
        {
            var response = await mediator.Send(new GetCommentQueryRequest { CommentId = commentId });

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsCount(
        [FromQuery] int? authorId,
        [FromQuery] int? movieId,
        [FromQuery] string? searchText
        )
        {
            var request = new GetCommentsCountQueryRequest();

            if (authorId.HasValue)
                request.predicate = request.predicate.And(c => c.AuthorId == authorId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(c => c.MovieId == movieId.Value);

            if (!string.IsNullOrEmpty(searchText))
                request.predicate = request.predicate.And(c => c.Message.ToLower().Contains(searchText.ToLower()));

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMyCommentsCount()
        {
            var response = await mediator.Send(new GetCommentsCountQueryRequest());

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "Comment was created successfully!" });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "Comment was updated succesfully." });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        [Route("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await mediator.Send(new DeleteCommentCommandRequest { CommentId = commentId });

            return Ok(new { message = "Comment was deleted successfully!" });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> IncrementCommentLike([FromQuery] int commentId)
        {
            await mediator.Send(new IncrementCommentLikeCommandRequest { CommentId = commentId });

            return Ok(new { message = "Like count of the comment was incremented successfully."});
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> DecrementCommentLike([FromQuery] int commentId)
        {
            await mediator.Send(new DecrementCommentLikeCommandRequest { CommentId = commentId });

            return Ok(new { message = "Like count of the comment was decremented successfully." });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> IncrementCommentDislike([FromQuery] int commentId)
        {
            await mediator.Send(new IncrementCommentDislikeCommandRequest { CommentId = commentId });

            return Ok(new { message = "Dislike count of the comment was incremented successfully." });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> DecrementCommentDislike([FromQuery] int commentId)
        {
            await mediator.Send(new DecrementCommentDislikeCommandRequest { CommentId = commentId });

            return Ok(new { message = "Like count of the comment was decremented successfully." });
        }
    }
}