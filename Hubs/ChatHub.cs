using Microsoft.AspNetCore.SignalR;

using Microsoft.AspNetCore.SignalR;

namespace ChatR.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, string content, int conversationId)
        {
            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", senderId, content);
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
