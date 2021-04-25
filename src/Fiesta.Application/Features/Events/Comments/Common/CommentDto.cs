using System;
using Fiesta.Application.Features.Common;

namespace Fiesta.Application.Features.Events.Comments.Common
{
    public class CommentDto
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public UserDto Sender { get; set; }

        public int ReplyCount { get; set; }

        public bool IsEdited { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ParentId { get; set; }
    }
}
