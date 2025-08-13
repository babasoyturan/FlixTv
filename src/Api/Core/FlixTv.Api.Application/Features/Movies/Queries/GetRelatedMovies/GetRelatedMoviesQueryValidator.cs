using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRelatedMovies
{
    public class GetRelatedMoviesQueryValidator : AbstractValidator<GetRelatedMoviesQueryRequest>
    {
        public GetRelatedMoviesQueryValidator()
        {
            RuleFor(m => m.MovieId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(m => m.Count)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
