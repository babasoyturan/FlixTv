using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.FavouriteMovies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.CreateFavouriteMovie
{
    public class CreateFavouriteMovieCommandHandler : IRequestHandler<CreateFavouriteMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateFavouriteMovieCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(CreateFavouriteMovieCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<User>().GetAsync(u => u.Id == request.UserId) is null)
                throw new Exception("User was not found");

            if (await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId) is null)
                throw new Exception("Movie was not found");

            if (await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(f => f.UserId == request.UserId && f.MovieId == request.MovieId) is not null)
                throw new Exception("The favourite movie was already created.");

            var favouriteMovie = new UserMovieCatalog(request.UserId, request.MovieId);

            await unitOfWork.GetWriteRepository<UserMovieCatalog>().AddAsync(favouriteMovie);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
