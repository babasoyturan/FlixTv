using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.IncrementCommentLike
{
    public class IncrementCommentLikeCommandHandler : IRequestHandler<IncrementCommentLikeCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public IncrementCommentLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(IncrementCommentLikeCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId);

            if (comment is null)
                throw new Exception("Comment was not found.");

            comment.LikeCount++;
            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
