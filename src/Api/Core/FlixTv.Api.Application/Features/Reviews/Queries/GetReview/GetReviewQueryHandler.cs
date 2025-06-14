using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Comments;
using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetReview
{
    public class GetReviewQueryHandler : IRequestHandler<GetReviewQueryRequest, GetReviewQueryResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetReviewQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetReviewQueryResponse> Handle(GetReviewQueryRequest request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.GetReadRepository<Review>().GetAsync(r => r.Id == request.ReviewId, x => x.Include(c => c.Author).Include(c => c.Movie));

            if (review == null)
                throw new Exception("Review was not found.");

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetReviewQueryResponse, Review>(review);

            return response;
        }
    }
}
