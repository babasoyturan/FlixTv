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

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetMyFavouriteMoviesCount
{
    public class GetMyFavouriteMoviesCountQueryHandler : IRequestHandler<GetMyFavouriteMoviesCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public GetMyFavouriteMoviesCountQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<int> Handle(GetMyFavouriteMoviesCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<UserMovieCatalog>().CountAsync(x => x.UserId == userId);

            return response;
        }
    }
}
