using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixTv.Api.Application.Features.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public UpdateCommentCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(UpdateCommentCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.Id);

            if (comment is null)
                throw new Exception("Comment was not found.");

            if (comment.AuthorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this comment.");

            if (!string.IsNullOrWhiteSpace(request.Message) && comment.Message != request.Message)
                comment.Message = request.Message;

            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
