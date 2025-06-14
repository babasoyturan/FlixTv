using FlixTv.Common.Models.RequestModels.Reviews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommandRequest>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(r => r.AuthorId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.MovieId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.Message)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(r => r.Title)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(r => r.RatingPoint)
                .NotEmpty()
                .Must(r => r <= 10 && r >= 1)
                .WithMessage($"Rating point must be number which is between 1 and 10.");
        }
    }
}
