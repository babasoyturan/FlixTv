using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.DecrementCommentLike
{
    public class DecrementCommentLikeCommandHandler : IRequestHandler<DecrementCommentLikeCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DecrementCommentLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(DecrementCommentLikeCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId);

            if (comment is null)
                throw new Exception("Comment was not found.");

            if (comment.DislikeCount == 0)
                throw new Exception("Like count is 0.");

            comment.LikeCount--;
            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
