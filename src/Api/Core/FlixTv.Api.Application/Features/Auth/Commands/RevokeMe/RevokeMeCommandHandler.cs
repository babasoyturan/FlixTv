using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.RevokeMe
{
    public class RevokeMeCommandHandler : IRequestHandler<RevokeMeCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public RevokeMeCommandHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(RevokeMeCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new Exception("User not found.");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);

            return Unit.Value;
        }
    }
}
