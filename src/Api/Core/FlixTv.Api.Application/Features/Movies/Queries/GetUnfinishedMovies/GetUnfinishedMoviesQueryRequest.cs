using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetUnfinishedMovies
{
    public class GetUnfinishedMoviesQueryRequest : IRequest<IList<GetAllMoviesQueryResponse>>
    {
    }
}
