using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
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
        private readonly IMapper mapper;

        public DeleteFavouriteMovieCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteFavouriteMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var favouriteMovie = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(c => c.Id == request.FavouriteMovieId);

            if (favouriteMovie is null)
                throw new Exception("The favourite movie was not found.");

            await unitOfWork.GetWriteRepository<UserMovieCatalog>().DeleteAsync(favouriteMovie);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
