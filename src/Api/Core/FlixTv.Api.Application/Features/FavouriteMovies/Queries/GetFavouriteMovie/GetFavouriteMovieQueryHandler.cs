using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Comments;
using FlixTv.Common.Models.ResponseModels.FavouriteMovies;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetFavouriteMovie
{
    public class GetFavouriteMovieQueryHandler : IRequestHandler<GetFavouriteMovieQueryRequest, GetFavouriteMovieQueryResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetFavouriteMovieQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetFavouriteMovieQueryResponse> Handle(GetFavouriteMovieQueryRequest request, CancellationToken cancellationToken)
        {
            var favouriteMovie = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(c => c.Id == request.FavouriteMovieId, x => x.Include(c => c.Movie).ThenInclude(m  => m.Reviews));

            if (favouriteMovie == null)
                throw new Exception("The favourite movie was not found.");

            mapper.Map<FavouriteMovieDto, Movie>(new Movie());

            var response = mapper.Map<GetFavouriteMovieQueryResponse, UserMovieCatalog>(favouriteMovie);

            response.Movie.Rating = favouriteMovie.Movie.GetMovieRating();

            return response;
        }
    }
}
