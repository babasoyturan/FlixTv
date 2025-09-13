using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.ToggleFavouriteMovie
{
    public class ToggleFavouriteMovieCommandRequest : IRequest<Unit>
    {
        public int MovieId { get; set; }
    }
}
