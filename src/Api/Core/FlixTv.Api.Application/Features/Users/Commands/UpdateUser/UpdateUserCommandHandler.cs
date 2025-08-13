using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.RequestModels.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public UpdateUserCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new Exception($"User with ID {userId} not found.");

            if (!string.IsNullOrEmpty(request.Name) && request.Name != user.Name)
                user.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Surname) && request.Surname != user.Surname)
                user.Surname = request.Surname;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                    throw new Exception("A user with this email already exists.");

                user.Email = request.Email;
                user.UserName = request.Email;
                user.EmailConfirmed = false;

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                await userManager.UpdateSecurityStampAsync(user);

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var param = new Dictionary<string, string>
                {
                    { "token", token },
                    { "email", user.Email }
                };

                var callback = QueryHelpers.AddQueryString("https://localhost:5000/api/Auth/EmailConfirmation", param);

                QueueFactory.SendMessageToExchange(
                exchangeName: FlixTvConstants.MovieExchangeName,
                exchangeType: FlixTvConstants.DefaultExchangeType,
                queueName: FlixTvConstants.SendEmailQueueName,
                obj: new EmailMessageDto
                {
                    To = request.Email,
                    Subject = "Email Confirmation Token",
                    Body = callback
                });
            }

            await userManager.UpdateAsync(user);

            return Unit.Value;
        }
    }
}
