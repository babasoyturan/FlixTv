using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeleteReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.GetReadRepository<Review>().GetAsync(c => c.Id == request.ReviewId);

            if (review is null)
                throw new Exception("Review was not found.");

            await unitOfWork.GetWriteRepository<Review>().DeleteAsync(review);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
