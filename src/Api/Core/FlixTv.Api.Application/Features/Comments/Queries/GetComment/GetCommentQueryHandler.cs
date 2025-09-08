using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetComment
{
    public class GetCommentQueryHandler : IRequestHandler<GetCommentQueryRequest, GetCommentQueryResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public GetCommentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<GetCommentQueryResponse> Handle(GetCommentQueryRequest request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(c => c.Id == request.CommentId, x => x.Include(c => c.Author).Include(c => c.Movie));

            if (comment == null)
                throw new Exception("Comment was not found.");

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetCommentQueryResponse, Comment>(comment);

            response.LikeCount = comment.Likes.Count;
            response.DislikeCount = comment.Dislikes.Count;
            response.HasLiked = userId == 0 ? false : comment.Likes.Contains(userId);
            response.HasDisliked = userId == 0 ? false : comment.Dislikes.Contains(userId);

            return response;
        }
    }
}
