using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Behaviours.Authorization;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Common;
using Fiesta.Application.Features.Events.Comments.Common;
using Fiesta.Application.Features.Events.Common;
using Fiesta.Application.Utils;
using Fiesta.Domain.Entities.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fiesta.Application.Features.Events.Comments
{
    public class CreateOrUpdateComment
    {
        public class Command : IRequest<CommentDto>
        {
            [JsonIgnore]
            public string EventId { get; set; }

            [JsonIgnore]
            public string CurrentUserId { get; set; }

            [JsonIgnore]
            public string CommentId { get; set; }

            public string Text { get; set; }

            public string ParentId { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommentDto>
        {
            private readonly IFiestaDbContext _db;

            public Handler(IFiestaDbContext db)
            {
                _db = db;
            }

            public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
            {
                EventComment comment;
                if (request.CommentId == default)
                {
                    var @event = await _db.Events.FindOrNotFoundAsync(cancellationToken, request.EventId);
                    var sender = await _db.FiestaUsers.FindAsync(new[] { request.CurrentUserId }, cancellationToken);
                    var parent = string.IsNullOrEmpty(request.ParentId) ? null : await _db.EventComments.FindOrNotFoundAsync(cancellationToken, request.ParentId);

                    comment = new EventComment(request.Text, sender, @event, parent);
                    _db.EventComments.Add(comment);
                }
                else
                {
                    comment = await _db.EventComments.Include(x => x.Sender).SingleOrNotFoundAsync(x => x.Id == request.CommentId, cancellationToken);
                    comment.Edit(request.Text);
                }

                await _db.SaveChangesAsync(cancellationToken);

                return new CommentDto
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    CreatedAt = comment.CreatedAt,
                    IsEdited = comment.IsEdited,
                    ParentId = comment.ParentId,
                    ReplyCount = 0,
                    Sender = new UserDto
                    {
                        Id = comment.Sender.Id,
                        Username = comment.Sender.Username,
                        PictureUrl = comment.Sender.PictureUrl,
                        FirstName = comment.Sender.FirstName,
                        LastName = comment.Sender.LastName,
                    },
                };
            }
        }

        public class AuthorizationCheck : IAuthorizationCheck<Command>
        {
            public async Task<bool> IsAuthorized(Command request, IFiestaDbContext db, ICurrentUserService currentUserService, CancellationToken cancellationToken)
            {
                if (request.CommentId == default)
                    return await Helpers.IsOrganizerOrAttendeeOrAdmin(request.EventId, db, currentUserService, cancellationToken);

                var comment = await db.EventComments.FindOrNotFoundAsync(cancellationToken, request.CommentId);
                return currentUserService.IsResourceOwnerOrAdmin(comment.SenderId);
            }

        }

        public class Validator : AbstractValidator<Command>
        {
            private IFiestaDbContext _db;

            public Validator(IFiestaDbContext db)
            {
                _db = db;

                RuleFor(x => x.Text)
                    .NotEmpty().WithErrorCode(ErrorCodes.Required)
                    .MaximumLength(1000).WithErrorCode(ErrorCodes.MaxLength).WithState(_ => new { MaxLength = 1000 });

                RuleFor(x => x.ParentId).MustAsync(CannotBeReplyToReplyComment).WithErrorCode(ErrorCodes.InvalidOperation);
            }

            private async Task<bool> CannotBeReplyToReplyComment(string parentId, CancellationToken cancellationToken)
            {
                if (parentId is null)
                    return true;

                var parent = await _db.EventComments.SingleOrNotFoundAsync(x => x.Id == parentId, cancellationToken);
                return parent.ParentId is null;
            }
        }
    }
}
