using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.FavouriteMovies;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.CreateFavouriteMovie
{
    public class CreateFavouriteMovieCommandHandler : IRequestHandler<CreateFavouriteMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public CreateFavouriteMovieCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<Unit> Handle(CreateFavouriteMovieCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId) is null)
                throw new Exception("Movie was not found");

            if (await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(f => f.UserId == userId && f.MovieId == request.MovieId) is not null)
                throw new Exception("The favourite movie was already created.");

            var favouriteMovie = new UserMovieCatalog(userId, request.MovieId);

            await unitOfWork.GetWriteRepository<UserMovieCatalog>().AddAsync(favouriteMovie);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
