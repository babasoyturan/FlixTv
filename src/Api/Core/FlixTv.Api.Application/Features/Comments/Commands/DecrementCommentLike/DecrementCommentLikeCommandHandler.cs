using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.DecrementCommentLike
{
    public class DecrementCommentLikeCommandHandler : IRequestHandler<DecrementCommentLikeCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public DecrementCommentLikeCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(DecrementCommentLikeCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId);

            if (comment is null)
                throw new Exception("Comment was not found.");

            if (comment.Likes.Count() == 0)
                throw new Exception("Like count is 0.");

            if (!comment.Likes.Contains(userId))
                throw new Exception($"The User which Id is {userId}, was not liked the comment.");

            comment.Likes.Remove(userId);
            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
