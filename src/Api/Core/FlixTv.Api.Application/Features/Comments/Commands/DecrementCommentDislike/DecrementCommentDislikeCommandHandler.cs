﻿using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.DecrementCommentDislike
{
    public class DecrementCommentDislikeCommandHandler : IRequestHandler<DecrementCommentDislikeCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DecrementCommentDislikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(DecrementCommentDislikeCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId);

            if (comment is null)
                throw new Exception("Comment was not found.");

            if (comment.DislikeCount == 0)
                throw new Exception("Dislike count is 0.");

            comment.DislikeCount--;
            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
