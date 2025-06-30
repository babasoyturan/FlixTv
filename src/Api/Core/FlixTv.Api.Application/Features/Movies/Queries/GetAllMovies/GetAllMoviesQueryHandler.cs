using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetAllMovies
{
    public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQueryRequest, IList<GetAllMoviesQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetAllMoviesQueryResponse>> Handle(GetAllMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            IList<Movie> movies = new List<Movie>();
            if (request.currentPage > 0 && request.pageSize > 0)
                movies = await unitOfWork.GetReadRepository<Movie>().GetAllByPagingAsync(
                    request.predicate,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(m => m.Reviews).Include(m => m.Views));
            else
                movies = await unitOfWork.GetReadRepository<Movie>().GetAllAsync(
                    request.predicate,
                    request.orderBy,
                    x => x.Include(m => m.Reviews).Include(m => m.Views));

            var response = mapper.Map<GetAllMoviesQueryResponse, Movie>(movies);

            for (int i = 0; i < response.Count(); i++)
            {
                response[i].ViewCount = movies[i].Views.Count();

                if (request.userId == 0)
                {
                    response[i].IsFavourite = false;
                    continue;
                }

                var fm = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(
                    x => x.MovieId == movies[i].Id && x.UserId == request.userId);

                response[i].IsFavourite = fm is not null;
            }
                

            return response;
        }
    }
}
