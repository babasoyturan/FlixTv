using FlixTv.Common.Models.RequestModels.FavouriteMovies;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.CreateFavouriteMovie
{
    public class CreateFavouriteMovieCommandValidator : AbstractValidator<CreateFavouriteMovieCommandRequest>
    {
        public CreateFavouriteMovieCommandValidator()
        {
            RuleFor(f => f.MovieId)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
