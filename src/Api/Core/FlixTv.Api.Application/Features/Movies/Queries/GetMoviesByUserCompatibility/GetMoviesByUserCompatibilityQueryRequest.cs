using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMoviesByUserCompatibility
{
    public class GetMoviesByUserCompatibilityQueryRequest : IRequest<IList<GetAllMoviesQueryResponse>>
    {
        public Expression<Func<Movie, bool>> predicate { get; set; } = p => true;

        public int userId { get; set; }

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
