using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;

        public ForgotPasswordCommandHandler(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw new Exception("User with this email does not exist.");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                { "token", token },
                { "email", request.Email }
            };

            var callback = QueryHelpers.AddQueryString(request.ClientUri!, param!);

            QueueFactory.SendMessageToExchange(
                exchangeName: FlixTvConstants.MovieExchangeName,
                exchangeType: FlixTvConstants.DefaultExchangeType,
                queueName: FlixTvConstants.SendEmailQueueName,
                obj: new EmailMessageDto
                {
                    To = request.Email,
                    Subject = "Reset Password Token",
                    Body = callback
                });

            return Unit.Value;
        }
    }
}
