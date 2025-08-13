using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public DeleteUserCommandHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
                throw new Exception($"User with ID {request.UserId} not found.");

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                throw new Exception("You cannot delete an admin user.");

            var currentUser = await userManager.FindByIdAsync(userId.ToString());
            var currentUserRoles = await userManager.GetRolesAsync(currentUser);

            if (!currentUserRoles.Contains("Admin") && roles.Contains("Moderator"))
                throw new Exception("You do not have permission to delete a moderator user.");

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to delete user: {errors}");
            }

            return Unit.Value;
        }
    }
}
