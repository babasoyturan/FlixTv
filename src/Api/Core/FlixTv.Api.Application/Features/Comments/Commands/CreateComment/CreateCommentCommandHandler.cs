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

namespace FlixTv.Api.Application.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public CreateCommentCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(CreateCommentCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<User>().GetAsync(u => u.Id == userId) is null)
                throw new Exception("Author was not found");

            if (await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId) is null)
                throw new Exception("Movie was not found");

            Comment comment = new(userId, request.MovieId, request.Message);

            await unitOfWork.GetWriteRepository<Comment>().AddAsync(comment);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
