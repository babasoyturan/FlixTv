using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetMyViewDatas
{
    public class GetMyViewDatasQueryHandler : IRequestHandler<GetMyViewDatasQueryRequest, IList<GetViewDataQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public GetMyViewDatasQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<IList<GetViewDataQueryResponse>> Handle(GetMyViewDatasQueryRequest request, CancellationToken cancellationToken)
        {
            IList<ViewData> viewDatas = new List<ViewData>();
            if (request.currentPage > 0 && request.pageSize > 0)
                viewDatas = await unitOfWork.GetReadRepository<ViewData>().GetAllByPagingAsync(
                    x => x.UserId == userId,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(vd => vd.Movie).Include(vd => vd.User));
            else
                viewDatas = await unitOfWork.GetReadRepository<ViewData>().GetAllAsync(
                    x => x.UserId == userId,
                    request.orderBy,
                    x => x.Include(vd => vd.Movie).Include(vd => vd.User));

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetViewDataQueryResponse, ViewData>(viewDatas);

            return response;
        }
    }
}
