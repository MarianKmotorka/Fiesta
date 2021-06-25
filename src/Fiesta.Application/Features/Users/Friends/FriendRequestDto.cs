using System;
using Fiesta.Application.Features.Common;

namespace Fiesta.Application.Features.Users.Friends
{
    public class FriendRequestDto
    {
        public UserDto User { get; set; }

        public DateTime RequestedOn { get; set; }
    }
}
