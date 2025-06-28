using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRelatedMovies
{
    public class GetRelatedMoviesQueryRequest : IRequest<IList<GetAllMoviesQueryResponse>>
    {
        public int MovieId { get; set; }
        public int Size { get; set; }
    }
}
