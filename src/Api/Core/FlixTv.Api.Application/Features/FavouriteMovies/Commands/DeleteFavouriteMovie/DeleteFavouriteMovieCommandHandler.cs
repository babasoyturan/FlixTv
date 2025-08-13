using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.DeleteFavouriteMovie
{
    public class DeleteFavouriteMovieCommandHandler : IRequestHandler<DeleteFavouriteMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public DeleteFavouriteMovieCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "0");
        }

        public async Task<Unit> Handle(DeleteFavouriteMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var favouriteMovie = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(c => c.Id == request.FavouriteMovieId);

            if (favouriteMovie is null)
                throw new Exception("The favourite movie was not found.");

            if (favouriteMovie.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own favourite movie.");

            await unitOfWork.GetWriteRepository<UserMovieCatalog>().DeleteAsync(favouriteMovie);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
