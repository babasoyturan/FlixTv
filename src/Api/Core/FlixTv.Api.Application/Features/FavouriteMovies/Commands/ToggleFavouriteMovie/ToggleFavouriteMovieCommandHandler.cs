using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.ToggleFavouriteMovie
{
    public class ToggleFavouriteMovieCommandHandler : IRequestHandler<ToggleFavouriteMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public ToggleFavouriteMovieCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<Unit> Handle(ToggleFavouriteMovieCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId) is null)
                throw new Exception("Movie was not found");

            var fm = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(f => f.UserId == userId && f.MovieId == request.MovieId);

            if (fm is null)
            {
                var favouriteMovie = new UserMovieCatalog(userId, request.MovieId);

                await unitOfWork.GetWriteRepository<UserMovieCatalog>().AddAsync(favouriteMovie);
            }
            else
            {
                await unitOfWork.GetWriteRepository<UserMovieCatalog>().DeleteAsync(fm);
            }


            await unitOfWork.SaveAsync();


            return Unit.Value;
        }
    }
}
