using FlixTv.Common.Models.ResponseModels.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryRequest : IRequest<GetUserQueryResponse>
    {
    }
}
