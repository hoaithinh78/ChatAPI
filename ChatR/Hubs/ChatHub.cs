using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using ChatR.Services.Interfaces;

namespace ChatR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IOnlineUserManager _onlineUserManager;

        public ChatHub(IOnlineUserManager onlineUserManager)
        {
            _onlineUserManager = onlineUserManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                _onlineUserManager.AddConnection(userId, Context.ConnectionId);
                await Clients.All.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            _onlineUserManager.RemoveConnection(Context.ConnectionId);

            if (userId.HasValue && !_onlineUserManager.IsUserOnline(userId.Value))
            {
                await Clients.All.SendAsync("UserOffline", userId.Value);
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
    }
}