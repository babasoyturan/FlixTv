using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryRequest : IRequest<IList<GetAllUsersQueryResponse>>
    {
        public Expression<Func<User, bool>> predicate { get; set; } = p => true;

        public Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
