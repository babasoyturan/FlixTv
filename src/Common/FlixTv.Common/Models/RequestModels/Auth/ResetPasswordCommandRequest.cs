using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Auth
{
    public class ResetPasswordCommandRequest : IRequest<Unit>
    {
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
