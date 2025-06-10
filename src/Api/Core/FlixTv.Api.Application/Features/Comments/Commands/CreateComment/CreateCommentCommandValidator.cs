using FlixTv.Common.Models.RequestModels.Comments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommandRequest>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(c => c.AuthorId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(c => c.MovieId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(c => c.Message)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}
