using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models;
using FlixTv.Common.Models.RequestModels.Movies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Commands.UpdateMovie
{
    public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateMovieCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.Id);

            if (movie is null)
                throw new Exception("The movie was not found.");

            if (!string.IsNullOrWhiteSpace(request.Title) && movie.Title != request.Title)
                movie.Title = request.Title;

            if (!string.IsNullOrWhiteSpace(request.Description) && movie.Description != request.Description)
                movie.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.TrailerVideoUrl) && movie.TrailerVideoUrl != request.TrailerVideoUrl)
                movie.TrailerVideoUrl = request.TrailerVideoUrl;

            if (!string.IsNullOrWhiteSpace(request.SourceVideoUrl) && movie.SourceVideoUrl != request.SourceVideoUrl)
                movie.SourceVideoUrl = request.SourceVideoUrl;

            if (request.ReleaseYear.HasValue && movie.ReleaseYear != request.ReleaseYear.Value)
                movie.ReleaseYear = request.ReleaseYear.Value;

            if (request.Duration.HasValue && movie.Duration != request.Duration.Value)
                movie.Duration = request.Duration.Value;

            if (request.AgeLimitation.HasValue && movie.AgeLimitation != request.AgeLimitation.Value)
                movie.AgeLimitation = request.AgeLimitation.Value;

            if (request.IsVisible.HasValue && movie.IsVisible != request.IsVisible.Value)
                movie.IsVisible = request.IsVisible.Value;

            if (request.Categories is not null && request.Categories.Count > 0 && request.Categories.All(c => c.HasValue && !string.IsNullOrWhiteSpace(c.Value.ToString())) )
            {
                movie.Categories.Clear();
                foreach (var category in request.Categories)
                    movie.Categories.Add(category.Value);
            }

            await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(movie);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
