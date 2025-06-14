using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Reviews;
using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetAllViewDatas
{
    public class GetAllViewDatasQueryHandler : IRequestHandler<GetAllViewDatasQueryRequest, IList<GetViewDataQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllViewDatasQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetViewDataQueryResponse>> Handle(GetAllViewDatasQueryRequest request, CancellationToken cancellationToken)
        {
            IList<ViewData> viewDatas = new List<ViewData>();
            if (request.currentPage > 0 && request.pageSize > 0)
                viewDatas = await unitOfWork.GetReadRepository<ViewData>().GetAllByPagingAsync(
                    request.predicate,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize);
            else
                viewDatas = await unitOfWork.GetReadRepository<ViewData>().GetAllAsync(
                    request.predicate,
                    request.orderBy);

            var response = mapper.Map<GetViewDataQueryResponse, ViewData>(viewDatas);

            return response;
        }
    }
}
