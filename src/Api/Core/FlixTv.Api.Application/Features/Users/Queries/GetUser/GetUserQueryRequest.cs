using FlixTv.Common.Models.ResponseModels.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetUser
{
    public class GetUserQueryRequest : IRequest<GetUserQueryResponse>
    {
        public int UserId { get; set; }
    }
}
