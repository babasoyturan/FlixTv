﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Comments
{
    public class UpdateCommentCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}