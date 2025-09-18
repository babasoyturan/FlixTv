using FlixTv.Api.Application.Features.Reviews.Commands.DeleteReview;
using FlixTv.Api.Application.Features.Reviews.Queries.GetAllReviews;
using FlixTv.Api.Application.Features.Reviews.Queries.GetReview;
using FlixTv.Api.Application.Features.ViewDatas.Commands.DeleteViewData;
using FlixTv.Api.Application.Features.ViewDatas.Queries.GetAllViewDatas;
using FlixTv.Api.Application.Features.ViewDatas.Queries.GetMyViewDatas;
using FlixTv.Api.Application.Features.ViewDatas.Queries.GetMyViewDatasCount;
using FlixTv.Api.Application.Features.ViewDatas.Queries.GetViewData;
using FlixTv.Api.Application.Features.ViewDatas.Queries.GetViewDatasCount;
using FlixTv.Api.Application.Utilities;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.ViewDatas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ViewDatasController : ControllerBase
    {
        private readonly IMediator mediator;

        public ViewDatasController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllViewDatas(
        [FromQuery] int? userId,
        [FromQuery] int? movieId,
        [FromQuery] DateTime? minDate,
        [FromQuery] DateTime? maxDate,
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetAllViewDatasQueryRequest()
            {
                pageSize = pageSize,
                currentPage = currentPage,
            };

            if (userId.HasValue)
                request.predicate = request.predicate.And(v => v.UserId == userId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(v => v.MovieId == movieId.Value);

            if (minDate.HasValue)
                request.predicate = request.predicate.And(v => v.CreatedDate >= minDate.Value);

            if (maxDate.HasValue)
                request.predicate = request.predicate.And(v => v.CreatedDate <= maxDate.Value);

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
        [Route("{viewDataId}")]
        public async Task<IActionResult> GetViewData(int viewDataId)
        {
            var response = await mediator.Send(new GetViewDataQueryRequest { ViewDataId = viewDataId });

            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetViewDatasCount(
        [FromQuery] int? userId,
        [FromQuery] int? movieId,
        [FromQuery] DateTime? minDate,
        [FromQuery] DateTime? maxDate
        )
        {
            var request = new GetViewDatasCountQueryRequest();

            if (userId.HasValue)
                request.predicate = request.predicate.And(v => v.UserId == userId.Value);

            if (movieId.HasValue)
                request.predicate = request.predicate.And(v => v.MovieId == movieId.Value);

            if (minDate.HasValue)
                request.predicate = request.predicate.And(v => v.CreatedDate >= minDate.Value);

            if (maxDate.HasValue)
                request.predicate = request.predicate.And(v => v.CreatedDate <= maxDate.Value);

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMyViewDatas(
        [FromQuery] string? orderBy,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 0
        )
        {
            var request = new GetMyViewDatasQueryRequest()
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

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetMyViewDatasCount()
        {
            var request = new GetMyViewDatasCountQueryRequest();

            var response = await mediator.Send(request);

            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator, User")]
        [HttpPost]
        public async Task<IActionResult> CreateViewData([FromBody] CreateViewDataCommandRequest request)
        {
            await mediator.Send(request);

            return Ok("View Data was created succesfully.");
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [Route("{viewDataId}")]
        public async Task<IActionResult> DeleteViewData(int viewDataId)
        {
            await mediator.Send(new DeleteViewDataCommandRequest { ViewDataId = viewDataId });

            return Ok("View Data was deleted successfully!");
        }
    }
}
