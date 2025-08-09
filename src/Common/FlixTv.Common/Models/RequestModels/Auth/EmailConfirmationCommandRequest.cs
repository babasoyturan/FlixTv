using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Auth
{
    public class EmailConfirmationCommandRequest : IRequest<Unit>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
