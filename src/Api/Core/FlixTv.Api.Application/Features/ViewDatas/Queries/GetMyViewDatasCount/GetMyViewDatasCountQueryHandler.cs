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

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetMyViewDatasCount
{
    public class GetMyViewDatasCountQueryHandler : IRequestHandler<GetMyViewDatasCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public GetMyViewDatasCountQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<int> Handle(GetMyViewDatasCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<ViewData>().CountAsync(x => x.UserId == userId);

            return response;
        }
    }
}
