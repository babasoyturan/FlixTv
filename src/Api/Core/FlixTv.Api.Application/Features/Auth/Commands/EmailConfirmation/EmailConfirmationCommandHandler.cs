using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.EmailConfirmation
{
    public class EmailConfirmationCommandHandler : IRequestHandler<EmailConfirmationCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;

        public EmailConfirmationCommandHandler(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(EmailConfirmationCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            
            if (user is null)
                throw new Exception("Invalid Email Confirmation Request.");

            var confirmResult = await userManager.ConfirmEmailAsync(user, request.Token);
            if (!confirmResult.Succeeded)
                throw new Exception("Email confirmation failed. Please try again.");

            return Unit.Value;
        }
    }
}
