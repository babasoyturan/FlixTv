using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Reviews;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public UpdateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(UpdateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.GetReadRepository<Review>().GetAsync(c => c.Id == request.Id);
            bool isRatingChanged = false;

            if (review is null)
                throw new Exception("Review was not found.");

            if (review.AuthorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this review.");

            if (!string.IsNullOrWhiteSpace(request.Message))
                review.Message = request.Message;

            if (!string.IsNullOrWhiteSpace(request.Title))
                review.Title = request.Title;

            if (request.RatingPoint >= 0)
            {
                review.RatingPoint = request.RatingPoint;
                isRatingChanged = true;
            }

            await unitOfWork.GetWriteRepository<Review>().UpdateAsync(review);

            await unitOfWork.SaveAsync();

            if (!isRatingChanged)
            {
                var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == review.MovieId);
                movie.SetMovieRating();
                await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(movie);
                await unitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
