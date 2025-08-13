using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public DeleteReviewCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "0");
        }

        public async Task<Unit> Handle(DeleteReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.GetReadRepository<Review>().GetAsync(c => c.Id == request.ReviewId);

            if (review is null)
                throw new Exception("Review was not found.");

            var user = await userManager.FindByIdAsync(userId.ToString());

            var roles = await userManager.GetRolesAsync(user!);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && userId != review.AuthorId)
                throw new UnauthorizedAccessException("You are not authorized to delete this review.");

            await unitOfWork.GetWriteRepository<Review>().DeleteAsync(review);
            await unitOfWork.SaveAsync();

            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == review.MovieId);
            movie.SetMovieRating();
            await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(movie);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
