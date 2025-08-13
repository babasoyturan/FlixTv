using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Reviews;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        public readonly int userId;

        public CreateReviewCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId, x => x.Include(m => m.Reviews));

            if (movie is null)
                throw new Exception("Movie was not found");

            var review = new Review(userId, request.MovieId, request.Title, request.Message, request.RatingPoint);

            await unitOfWork.GetWriteRepository<Review>().AddAsync(review);

            await unitOfWork.SaveAsync();

            movie.Reviews.Add(review);

            movie.SetMovieRating();

            await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(movie);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
