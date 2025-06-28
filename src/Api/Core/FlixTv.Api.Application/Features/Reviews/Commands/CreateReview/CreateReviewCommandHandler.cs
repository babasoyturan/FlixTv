using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Reviews;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<User>().GetAsync(u => u.Id == request.AuthorId) is null)
                throw new Exception("Author was not found");

            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId);

            if (movie is null)
                throw new Exception("Movie was not found");

            var review = new Review(request.AuthorId, request.MovieId, request.Title, request.Message, request.RatingPoint);

            await unitOfWork.GetWriteRepository<Review>().AddAsync(review);

            await unitOfWork.SaveAsync();

            movie.SetMovieRating();

            await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(movie);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
