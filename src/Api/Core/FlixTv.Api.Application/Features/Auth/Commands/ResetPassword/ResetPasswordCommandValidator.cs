using FlixTv.Common.Models.RequestModels.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommandRequest>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MinimumLength(10).WithMessage("Token must be at least 10 characters long.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters.");

            RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm Password is required.")
                .Equal(u => u.NewPassword)
                .WithMessage("Confirm Password must match Password.")
                .MaximumLength(100)
                .WithMessage("Confirm Password must not exceed 100 characters.");
        }
    }
}
