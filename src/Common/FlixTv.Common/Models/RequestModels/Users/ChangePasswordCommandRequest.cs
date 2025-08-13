using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Users
{
    public class ChangePasswordCommandRequest : IRequest<Unit>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
