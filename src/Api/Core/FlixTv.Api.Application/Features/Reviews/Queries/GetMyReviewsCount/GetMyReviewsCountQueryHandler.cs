using FlixTv.Api.Application.Features.Reviews.Queries.GetReviewsCount;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetMyReviewsCount
{
    public class GetMyReviewsCountQueryHandler : IRequestHandler<GetMyReviewsCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public GetMyReviewsCountQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<int> Handle(GetMyReviewsCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<Review>().CountAsync(x => x.AuthorId == userId);

            return response;
        }
    }
}
