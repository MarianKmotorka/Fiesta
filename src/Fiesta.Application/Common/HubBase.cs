using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Fiesta.Application.Common
{
    public class HubBase<T> : Hub<T> where T : class
    {
        protected readonly ICurrentUserService _currentUser;
        private static ConcurrentDictionary<string, List<string>> _userConnections = new();

        public HubBase(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public static IReadOnlyDictionary<string, List<string>> UserConnections => _userConnections;

        public override async Task OnConnectedAsync()
        {
            _userConnections.AddOrUpdate(
               _currentUser.UserId,
               _ => new List<string> { Context.ConnectionId },
               (_, prev) => prev.Append(Context.ConnectionId).ToList()
               );

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _currentUser.UserId;

            if (!_userConnections.ContainsKey(userId))
                return;

            _userConnections[userId].Remove(Context.ConnectionId);

            if (_userConnections[userId].Count == 0)
                _userConnections.TryRemove(userId, out _);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
