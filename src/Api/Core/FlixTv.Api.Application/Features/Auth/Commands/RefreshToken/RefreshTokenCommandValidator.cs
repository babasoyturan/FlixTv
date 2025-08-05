using FlixTv.Common.Models.RequestModels.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommandRequest>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(m => m.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token cannot be empty.")
                .NotNull()
                .WithMessage("Refresh token cannot be null.");

            RuleFor(m => m.AccessToken)
                .NotEmpty()
                .WithMessage("Access token cannot be empty.")
                .NotNull()
                .WithMessage("Access token cannot be null.");
        }
    }
}
