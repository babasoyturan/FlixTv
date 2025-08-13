using FlixTv.Api.Application.Features.Reviews.Queries.GetAllReviews;
using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetMyReviews
{
    public class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQueryRequest, IList<GetReviewQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public GetMyReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<IList<GetReviewQueryResponse>> Handle(GetMyReviewsQueryRequest request, CancellationToken cancellationToken)
        {
            IList<Review> reviews = new List<Review>();
            if (request.currentPage > 0 && request.pageSize > 0)
                reviews = await unitOfWork.GetReadRepository<Review>().GetAllByPagingAsync(
                    x => x.AuthorId == userId,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(c => c.Author).Include(c => c.Movie));
            else
                reviews = await unitOfWork.GetReadRepository<Review>().GetAllAsync(
                    x => x.AuthorId == userId,
                    request.orderBy,
                    x => x.Include(c => c.Author).Include(c => c.Movie));

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetReviewQueryResponse, Review>(reviews);

            return response;
        }
    }
}
