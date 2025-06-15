using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.FavouriteMovies;
using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetAllFavouriteMovies
{
    public class GetAllFavouriteMoviesQueryHandler : IRequestHandler<GetAllFavouriteMoviesQueryRequest, IList<GetFavouriteMovieQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllFavouriteMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetFavouriteMovieQueryResponse>> Handle(GetAllFavouriteMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            IList<UserMovieCatalog> favouriteMovies = new List<UserMovieCatalog>();
            if (request.currentPage > 0 && request.pageSize > 0)
                favouriteMovies = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAllByPagingAsync(
                    request.predicate,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(f => f.Movie).ThenInclude(m => m.Reviews));
            else
                favouriteMovies = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAllAsync(
                    request.predicate,
                    request.orderBy,
                    x => x.Include(f => f.Movie).ThenInclude(m => m.Reviews));

            favouriteMovies = favouriteMovies.Where(f => f.Movie.IsVisible).ToList();

            mapper.Map<FavouriteMovieDto, Movie>(new Movie());

            var response = mapper.Map<GetFavouriteMovieQueryResponse, UserMovieCatalog>(favouriteMovies);

            for (int i = 0; i < response.Count(); i++)
                response[0].Movie.Rating = favouriteMovies[0].Movie.GetMovieRating();

            return response;
        }
    }
}
