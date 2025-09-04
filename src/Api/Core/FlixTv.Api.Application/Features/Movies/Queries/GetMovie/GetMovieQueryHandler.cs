using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Movies;
using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMovie
{
    public class GetMovieQueryHandler : IRequestHandler<GetMovieQueryRequest, GetMovieQueryResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public GetMovieQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<GetMovieQueryResponse> Handle(GetMovieQueryRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId);

            if (movie is null)
                throw new Exception("Movie was not found.");

            var response = mapper.Map<GetMovieQueryResponse, Movie>(movie);

            if (userId > 0)
            {
                var vd = await unitOfWork.GetReadRepository<ViewData>().GetAsync(v => v.MovieId == movie.Id && v.UserId == userId);
                if (vd is not null && !vd.IsCompleted)
                {
                    response.LastPositionSeconds = vd.LastPositionSeconds;
                }
            }

            response.SourceVideoUrl = $"https://{FlixTvConstants.CdnName}.cloudfront.net/{movie.SourceVideoUrl}/hls/{movie.SourceVideoUrl}.m3u8";
            response.SubtitleUrl = $"https://{FlixTvConstants.CdnName}.cloudfront.net/subtitles/{movie.SourceVideoUrl}.vtt";

            return response;
        }
    }
}
