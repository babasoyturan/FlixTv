using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Commands.DeleteMovie
{
    public class DeleteMovieCommandRequest : IRequest<Unit>
    {
        public int MovieId { get; set; }
    }
}
