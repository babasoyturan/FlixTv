using FlixTv.Api.Application.Features.Reviews.Queries.GetReview;
using FlixTv.Api.Application.Features.Users.Commands.DeleteUser;
using FlixTv.Api.Application.Features.Users.Queries.GetAllUsers;
using FlixTv.Api.Application.Features.Users.Queries.GetCurrentUser;
using FlixTv.Api.Application.Features.Users.Queries.GetUser;
using FlixTv.Api.Application.Features.Users.Queries.GetUsersCount;
using FlixTv.Api.Application.Utilities;
using FlixTv.Common.Models.RequestModels.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize( Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int? currentPage = 0,
            [FromQuery] int? pageSize = 0,
            [FromQuery] IList<string>? roles = null,
            [FromQuery] string? orderBy = null,
            [FromQuery] string? searchText = null
        )
        {
            var request = new GetAllUsersQueryRequest()
            {
                currentPage = currentPage ?? 0,
                pageSize = pageSize ?? 0
            };
            if (!string.IsNullOrEmpty(searchText))
                request.predicate = request.predicate.And(u => u.UserName.ToLower().Contains(searchText.ToLower()));
            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = "createdDate";

            switch (orderBy)
            {
                case "createdDate":
                    request.orderBy = x => x.OrderByDescending(u => u.CreatedDate);
                    break;
                case "name":
                    request.orderBy = x => x.OrderBy(u => u.Name);
                    break;
                default:
                    break;
            }
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await mediator.Send(new GetCurrentUserQueryRequest());
            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var response = await mediator.Send(new GetUserQueryRequest { UserId = userId });

            return Ok(response);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> GetUsersCount(
            [FromQuery] string? searchText = null
        )
        {
            var request = new GetUsersCountQueryRequest();

            if (!string.IsNullOrEmpty(searchText))
                request.predicate = request.predicate.And(u => u.UserName.ToLower().Contains(searchText.ToLower()));

            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "User updated successfully." });
        }

        [Authorize(Roles = "User, Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "Password changed successfully." });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [Route("{userId}")]
        public async Task<IActionResult> ChangeUserStatus(int userId)
        {
            var request = new ChangeUserStatusCommandRequest { UserId = userId };
            await mediator.Send(request);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetUserRole([FromBody] ResetUserRoleCommandRequest request)
        {
            await mediator.Send(request);

            return Ok(new { message = "User role reset successfully." });
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [Route("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var request = new DeleteUserCommandRequest { UserId = userId };
            await mediator.Send(request);
            return Ok(new { message = "User deleted successfully." });
        }
    }
}
