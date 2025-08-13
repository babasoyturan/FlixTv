using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetFavouriteMoviesCount
{
    public class GetFavouriteMoviesCountQueryHandler : IRequestHandler<GetFavouriteMoviesCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetFavouriteMoviesCountQueryHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(GetFavouriteMoviesCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<UserMovieCatalog>().CountAsync(request.predicate);

            return response;
        }
    }
}
