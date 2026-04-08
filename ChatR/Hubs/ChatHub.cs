using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using ChatR.Interface.Singleton;

namespace ChatR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IOnlineUserTracker _onlineUserTracker;

        public ChatHub(IOnlineUserTracker onlineUserTracker)
        {
            _onlineUserTracker = onlineUserTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                _onlineUserTracker.AddConnection(userId.Value, Context.ConnectionId);

                if (_onlineUserTracker.GetConnectionCount(userId.Value) == 1)
                {
                    await Clients.All.SendAsync("UserOnline", userId.Value);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                _onlineUserTracker.RemoveConnection(Context.ConnectionId);

                if (!_onlineUserTracker.IsUserOnline(userId.Value))
                {
                    await Clients.All.SendAsync("UserOffline", userId.Value);
                }
            }
            else
            {
                _onlineUserTracker.RemoveConnection(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int senderId, string content, int conversationId)
        {
            await Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", senderId, content);
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}