using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Commands.DeleteMovie
{
    public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteMovieCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId);

            if (movie is null)
                throw new Exception("The movie was not found.");

            await unitOfWork.GetWriteRepository<Movie>().DeleteAsync(movie);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
