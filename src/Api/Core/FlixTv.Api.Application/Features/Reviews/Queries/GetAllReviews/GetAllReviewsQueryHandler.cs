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

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetAllReviews
{
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQueryRequest, IList<GetReviewQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetReviewQueryResponse>> Handle(GetAllReviewsQueryRequest request, CancellationToken cancellationToken)
        {
            IList<Review> reviews = new List<Review>();
            if (request.currentPage > 0 && request.pageSize > 0)
                reviews = await unitOfWork.GetReadRepository<Review>().GetAllByPagingAsync(
                    request.predicate,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(c => c.Author).Include(c => c.Movie));
            else
                reviews = await unitOfWork.GetReadRepository<Review>().GetAllAsync(
                    request.predicate,
                    request.orderBy,
                    x => x.Include(c => c.Author).Include(c => c.Movie));

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetReviewQueryResponse, Review>(reviews);

            return response;
        }
    }
}
