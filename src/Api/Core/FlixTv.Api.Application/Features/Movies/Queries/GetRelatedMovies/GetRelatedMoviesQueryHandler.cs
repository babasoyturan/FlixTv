using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRelatedMovies
{
    public class GetRelatedMoviesQueryHandler : IRequestHandler<GetRelatedMoviesQueryRequest, IList<GetAllMoviesQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        private const double wCat = 1.0;
        private const double wYear = 0.1;
        private const double wAge = 0.2;

        public GetRelatedMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetAllMoviesQueryResponse>> Handle(GetRelatedMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId, x => x.Include(m => m.SimilarMovies).ThenInclude(m => m.Views));

            if (movie is null)
                throw new Exception("The movie is not found.");

            var relatedMovies = movie.SimilarMovies?.Where(m => m.IsVisible).ToList();

            if (relatedMovies is null || relatedMovies.Count() <= 0)
                throw new Exception("There are not any related movie.");

            var response = mapper.Map<GetAllMoviesQueryResponse, Movie>(relatedMovies);

            for (int i = 0; i < response.Count(); i++)
            {
                response[i].ViewCount = relatedMovies[i].Views.Count();

                if (request.UserId == 0)
                {
                    response[i].IsFavourite = false;
                    continue;
                }

                var fm = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(
                    x => x.MovieId == relatedMovies[i].Id && x.UserId == request.UserId);

                response[i].IsFavourite = fm is not null;
            }

            return response;
        }
    }
}
