using FlixTv.Common.Models;
using FlixTv.Common.Models.RequestModels.Movies;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Commands.CreateMovie
{
    public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommandRequest>
    {
        public CreateMovieCommandValidator()
        {
            RuleFor(m => m.AgeLimitation)
                .NotEmpty()
                .Must(a => a >= 0);

            RuleFor(m => m.Duration)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(m => m.ReleaseYear)
                .NotEmpty()
                .InclusiveBetween(1900, 3000)
                .WithMessage("Release year must be between 1900 and 3000.");

            RuleFor(m => m.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(m => m.Description)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(m => m.IsVisible)
                .NotNull()
                .WithMessage("IsVisible must be specified.");

            RuleFor(m => m.Categories)
                .NotEmpty()
                .WithMessage("At least one category must be specified.")
                .Must(categories => categories.All(c => Enum.IsDefined(typeof(MovieCategory), c)))
                .WithMessage("Invalid movie category specified.");

            RuleFor(m => m.TrailerVideoUrl)
                .NotEmpty()
                .WithMessage("Trailer video URL must not be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Trailer video URL must be a valid absolute URL.");

            RuleFor(m => m.SourceVideoUrl)
                .NotEmpty()
                .WithMessage("Source video url must not be empty.");

            RuleFor(RuleFor => RuleFor.CoverImageUrl)
                .NotEmpty()
                .WithMessage("Cover image URL must not be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Cover image URL must be a valid absolute URL.");

            RuleFor(RuleFor => RuleFor.BannerImageUrl)
                .NotEmpty()
                .WithMessage("Banner image URL must not be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Banner image URL must be a valid absolute URL.");


        }
    }
}
