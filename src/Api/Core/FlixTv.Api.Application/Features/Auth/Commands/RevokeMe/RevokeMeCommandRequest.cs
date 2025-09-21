using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.RevokeMe
{
    public class RevokeMeCommandRequest : IRequest<Unit>
    {
    }
}
