using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetViewDatasCount
{
    public class GetViewDatasCountQueryHandler : IRequestHandler<GetViewDatasCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetViewDatasCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<int> Handle(GetViewDatasCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<ViewData>().CountAsync(request.predicate);

            return response;
        }
    }
}
