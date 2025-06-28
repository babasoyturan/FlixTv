using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Movies;
using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId, x => x.Include(m => m.Reviews).ThenInclude(r => r.Author).Include(m => m.Comments).ThenInclude(c => c.Author));

            if (movie is null)
                throw new Exception("Movie was not found.");

            mapper.Map<AuthorDto, User>(new User());
            mapper.Map<CommentDto, Comment>(new Comment());
            mapper.Map<ReviewDto, Review>(new Review());

            var response = mapper.Map<GetMovieQueryResponse, Movie>(movie);

            response.Rating = movie.Rating;

            return response;
        }
    }
}
