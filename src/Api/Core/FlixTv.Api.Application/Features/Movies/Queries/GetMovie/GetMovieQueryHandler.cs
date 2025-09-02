using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Movies;
using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMovie
{
    public class GetMovieQueryHandler : IRequestHandler<GetMovieQueryRequest, GetMovieQueryResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetMovieQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetMovieQueryResponse> Handle(GetMovieQueryRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId);

            if (movie is null)
                throw new Exception("Movie was not found.");

            var response = mapper.Map<GetMovieQueryResponse, Movie>(movie);

            response.SourceVideoUrl = $"https://{FlixTvConstants.CdnName}.cloudfront.net/{movie.SourceVideoUrl}/hls/{movie.SourceVideoUrl}.m3u8";
            response.SubtitleUrl = $"https://{FlixTvConstants.CdnName}.cloudfront.net/{movie.SourceVideoUrl}/subtitles/{movie.SourceVideoUrl}.vtt";

            return response;
        }
    }
}
