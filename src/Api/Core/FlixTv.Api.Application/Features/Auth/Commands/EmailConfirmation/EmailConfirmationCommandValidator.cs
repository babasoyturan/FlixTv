using FlixTv.Common.Models.RequestModels.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.EmailConfirmation
{
    public class EmailConfirmationCommandValidator : AbstractValidator<EmailConfirmationCommandRequest>
    {
        public EmailConfirmationCommandValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(m => m.Token)
                .NotEmpty()
                .WithMessage("Token is required.")
                .MinimumLength(10)
                .WithMessage("Token must be at least 10 characters long.");
        }
    }
}
