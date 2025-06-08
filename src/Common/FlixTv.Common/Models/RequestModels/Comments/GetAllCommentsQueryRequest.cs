using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Comments
{
    public class GetAllCommentsQueryRequest : IRequest<IList<GetAllCommentsQueryResponse>>
    {

    }
}
