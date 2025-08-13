using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetUsersCount
{
    public class GetUsersCountQueryHandler : IRequestHandler<GetUsersCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetUsersCountQueryHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task<int> Handle(GetUsersCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = unitOfWork.GetReadRepository<User>().CountAsync(request.predicate);

            return response;
        }
    }
}
