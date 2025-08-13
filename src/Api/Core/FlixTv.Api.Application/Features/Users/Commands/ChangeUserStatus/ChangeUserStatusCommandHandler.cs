using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.ChangeUserStatus
{
    public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public ChangeUserStatusCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(ChangeUserStatusCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
                throw new Exception($"User with ID {request.UserId} not found.");

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                throw new UnauthorizedAccessException("You cannot change the status of an admin user.");

            var currentUser = await userManager.FindByIdAsync(userId.ToString());

            var currentUserRoles = await userManager.GetRolesAsync(currentUser);

            if (!currentUserRoles.Contains("Admin") && roles.Contains("Moderator"))
                throw new Exception("You do not have permission to change the status of a moderator user.");

            user.IsBanned = !user.IsBanned;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update user status: {errors}");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await userManager.UpdateAsync(user);

            await userManager.UpdateSecurityStampAsync(user);

            return Unit.Value;
        }
    }
}
