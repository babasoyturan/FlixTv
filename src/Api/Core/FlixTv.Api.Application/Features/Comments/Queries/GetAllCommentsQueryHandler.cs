using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.RequestModels.Comments;
using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries
{
    public class GetAllCommentsQueryHandler : IRequestHandler<GetAllCommentsQueryRequest, IList<GetAllCommentsQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllCommentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetAllCommentsQueryResponse>> Handle(GetAllCommentsQueryRequest request, CancellationToken cancellationToken)
        {
            var comments = await unitOfWork.GetReadRepository<Comment>().GetAllAsync(include: x => x.Include(c => c.Author).Include(c => c.Movie));

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetAllCommentsQueryResponse, Comment>(comments);

            return response;
        }
    }
}
