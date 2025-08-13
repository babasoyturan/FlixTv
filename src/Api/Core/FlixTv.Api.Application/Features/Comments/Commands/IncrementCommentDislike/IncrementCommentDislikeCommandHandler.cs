using FlixTv.Api.Application.Features.Comments.Commands.IncrementCommentLike;
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

namespace FlixTv.Api.Application.Features.Comments.Commands.IncrementCommentDislike
{
    public class IncrementCommentDislikeCommandHandler : IRequestHandler<IncrementCommentDislikeCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public IncrementCommentDislikeCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            userId = int.Parse(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(IncrementCommentDislikeCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId);

            if (comment is null)
                throw new Exception("Comment was not found.");

            if (comment.Dislikes.Contains(userId))
                throw new Exception($"The User which Id is {userId}, was already disliked the comment.");

            comment.Dislikes.Add(userId);
            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
