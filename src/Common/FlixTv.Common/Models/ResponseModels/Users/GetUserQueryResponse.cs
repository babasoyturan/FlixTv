using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.Users
{
    public class GetUserQueryResponse
    {
        public int Id { get; set; }
        public bool IsBanned { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
