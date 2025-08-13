using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQueryRequest, IList<GetAllUsersQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<IList<GetAllUsersQueryResponse>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
        {
            IList<User> users = new List<User>();
            if (request.currentPage > 0 && request.pageSize > 0)
                users = await unitOfWork.GetReadRepository<User>().GetAllByPagingAsync(
                    request.predicate,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(u => u.Comments).Include(u => u.Reviews));
            else
                users = await unitOfWork.GetReadRepository<User>().GetAllAsync(
                    request.predicate,
                    request.orderBy,
                    x => x.Include(u => u.Comments).Include(u => u.Reviews));


            var response = mapper.Map<GetAllUsersQueryResponse, User>(users);

            for (int i = 0; i < response.Count; i++)
            {
                response[i].Roles = await userManager.GetRolesAsync(users[i]);
                response[i].CommentCount = users[i].Comments.Count;
                response[i].ReviewCount = users[i].Reviews.Count;
            }

            return response;
        }
    }
}
