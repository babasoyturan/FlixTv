using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandRequest : IRequest<Unit>
    {
        public int UserId { get; set; }
    }
}
