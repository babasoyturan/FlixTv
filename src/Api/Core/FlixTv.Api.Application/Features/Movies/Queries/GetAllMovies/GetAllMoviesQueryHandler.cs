using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        private readonly int userId;

        public GetAllMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "0");
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

                if (userId == 0)
                {
                    response[i].IsFavourite = false;
                    continue;
                }

                var fm = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(
                    x => x.MovieId == movies[i].Id && x.UserId == userId);

                response[i].IsFavourite = fm is not null;
            }
                

            return response;
        }
    }
}
