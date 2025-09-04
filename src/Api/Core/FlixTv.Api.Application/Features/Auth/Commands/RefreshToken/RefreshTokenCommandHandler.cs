using FlixTv.Api.Application.Interfaces.Tokens;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Auth;
using FlixTv.Common.Models.ResponseModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly UserManager<User> userManager;

        public RefreshTokenCommandHandler(IConfiguration configuration, ITokenService tokenService, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.userManager = userManager;
        }

        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var email = principal?.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email);
            var roles = await userManager.GetRolesAsync(user);

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token or token has expired.");

            var newAccessToken = await tokenService.CreateToken(user, roles);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(configuration["JWT:RefreshTokenValidityInDays"]));

            await userManager.UpdateAsync(user);

            return new RefreshTokenCommandResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
                Expiration = user.RefreshTokenExpiryTime!.Value
            };
        }
    }
}
