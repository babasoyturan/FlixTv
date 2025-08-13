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
        public int count { get; set; }
    }
}
