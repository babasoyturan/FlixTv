using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.ToggleFavouriteMovie
{
    public class ToggleFavouriteMovieCommandValidator : AbstractValidator<ToggleFavouriteMovieCommandRequest>
    {
        public ToggleFavouriteMovieCommandValidator()
        {
            RuleFor(x => x.MovieId)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
