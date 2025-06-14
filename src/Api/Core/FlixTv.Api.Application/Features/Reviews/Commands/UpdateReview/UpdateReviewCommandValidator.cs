using FlixTv.Common.Models.RequestModels.Reviews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.UpdateReview
{
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommandRequest>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(r => r.Message)
                .MaximumLength(1000);

            RuleFor(r => r.Title)
                .MaximumLength(150);

            RuleFor(r => r.RatingPoint)
                .Must(r => r <= 10 && r >= 1)
                .WithMessage($"Rating point must be number which is between 1 and 10.");
        }
    }
}
