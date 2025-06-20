﻿using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(CreateCommentCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<User>().GetAsync(u => u.Id == request.AuthorId) is null)
                throw new Exception("Author was not found");

            if (await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId) is null)
                throw new Exception("Movie was not found");

            Comment comment = new(request.AuthorId, request.MovieId, request.Message);

            await unitOfWork.GetWriteRepository<Comment>().AddAsync(comment);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
