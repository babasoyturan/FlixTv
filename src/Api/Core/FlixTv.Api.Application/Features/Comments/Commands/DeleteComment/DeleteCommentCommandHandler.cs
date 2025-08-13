using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<User> userManager;
        private readonly int userId;

        public DeleteCommentCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "0");
        }

        public async Task<Unit> Handle(DeleteCommentCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId);

            if (comment is null)
                throw new Exception("Comment was not found.");

            var user = await userManager.FindByIdAsync(userId.ToString());

            var roles = await userManager.GetRolesAsync(user!);

            if (!roles.Contains("Admin") && !roles.Contains("Moderator") && userId != comment.AuthorId)
                throw new UnauthorizedAccessException("You are not authorized to delete this comment.");

            await unitOfWork.GetWriteRepository<Comment>().DeleteAsync(comment);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
