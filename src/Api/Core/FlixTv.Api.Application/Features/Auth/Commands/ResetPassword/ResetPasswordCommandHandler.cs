using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;

        public ResetPasswordCommandHandler(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email!);
            if (user is null)
                throw new Exception("User not found.");

            var result = await userManager.ResetPasswordAsync(user, request.Token!, request.NewPassword!);
            if (!result.Succeeded)
                throw new Exception("Invalid Reset Password Request.");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await userManager.UpdateAsync(user);


            await userManager.UpdateSecurityStampAsync(user);

            return Unit.Value;
        }
    }
}
