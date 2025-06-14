using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetReviewsCount
{
    public class GetReviewsCountQueryHandler : IRequestHandler<GetReviewsCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetReviewsCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<int> Handle(GetReviewsCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<Review>().CountAsync(request.predicate);

            return response;
        }
    }
}
