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

namespace FlixTv.Api.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public ChangePasswordCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            var changePasswordResult = await userManager.ChangePasswordAsync(user!, request.OldPassword, request.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to change password: {errors}");
            }

            return Unit.Value;
        }
    }
}
