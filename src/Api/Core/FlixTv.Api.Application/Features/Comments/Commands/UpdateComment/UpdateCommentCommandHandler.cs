using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixTv.Api.Application.Features.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UpdateCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCommentCommandRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.Id);

            if (comment is null)
                throw new Exception("Comment was not found.");

            if (!string.IsNullOrWhiteSpace(request.Message))
                comment.Message = request.Message;

            await unitOfWork.GetWriteRepository<Comment>().UpdateAsync(comment);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
