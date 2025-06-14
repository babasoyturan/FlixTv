using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetViewData
{
    public class GetViewDataQueryHandler : IRequestHandler<GetViewDataQueryRequest, GetViewDataQueryResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetViewDataQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetViewDataQueryResponse> Handle(GetViewDataQueryRequest request, CancellationToken cancellationToken)
        {
            var viewData = await unitOfWork.GetReadRepository<ViewData>().GetAsync(r => r.Id == request.ViewDataId);

            if (viewData == null)
                throw new Exception("View Data was not found.");

            var response = mapper.Map<GetViewDataQueryResponse, ViewData>(viewData);

            return response;
        }
    }
}
