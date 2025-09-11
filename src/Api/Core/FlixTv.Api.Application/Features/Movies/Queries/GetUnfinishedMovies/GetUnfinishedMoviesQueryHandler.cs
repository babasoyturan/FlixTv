using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetUnfinishedMovies
{
    public class GetUnfinishedMoviesQueryHandler : IRequestHandler<GetUnfinishedMoviesQueryRequest, IList<GetAllMoviesQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public GetUnfinishedMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<IList<GetAllMoviesQueryResponse>> Handle(GetUnfinishedMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            var movies = await unitOfWork.GetReadRepository<Movie>().GetAllAsync(
                    m => m.Views != null && m.Views.Any(v => v.UserId == userId &&
                         !v.IsCompleted &&
                         v.WatchedSeconds > 120),
                    x => x.OrderByDescending(m => m.Views.FirstOrDefault(v => v.UserId == userId).LastWatchedAt),
                    x => x.Include(m => m.Reviews).Include(m => m.Views));

            var response = mapper.Map<GetAllMoviesQueryResponse, Movie>(movies);

            var favs = await unitOfWork.GetReadRepository<UserMovieCatalog>()
                                       .GetAllAsync(x => x.UserId == userId, enableTracking: false);
            var favSet = new HashSet<int>(favs.Select(f => f.MovieId));

            for (int i = 0; i < response.Count(); i++)
            {
                var mv = movies[i];
                response[i].ViewCount = mv.Views?.Count() ?? 0;
                response[i].IsFavourite = favSet.Contains(mv.Id);
            }

            return response;
        }
    }
}
