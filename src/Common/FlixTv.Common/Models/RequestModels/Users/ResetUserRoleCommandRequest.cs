using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Users
{
    public class ResetUserRoleCommandRequest : IRequest<Unit>
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
