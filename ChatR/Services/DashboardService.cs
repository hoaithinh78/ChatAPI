using ChatR.Data;
using ChatR.DTOs.Dashboard;
using ChatR.DTOs.Message;
using ChatR.DTOs.Server;
using ChatR.DTOs.User;
using ChatR.Helpers;
using ChatR.Models;
using ChatR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatR.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int userId)
        {
            var myServerIds = await _context.ServerMembers
                .Where(sm => sm.UserId == userId)
                .Select(sm => sm.ServerId)
                .ToListAsync();

            var friendIds1 = await _context.Friends
                .Where(f => f.UserId == userId)
                .Select(f => f.FriendId)
                .ToListAsync();

            var friendIds2 = await _context.Friends
                .Where(f => f.FriendId == userId)
                .Select(f => f.UserId)
                .ToListAsync();

            var friendIds = friendIds1
                .Union(friendIds2)
                .Distinct()
                .ToList();

            var totalMessages = await _context.Messages
                .Where(m =>
                    (m.ChannelId != null && _context.Channels.Any(c => c.ChannelId == m.ChannelId && myServerIds.Contains(c.ServerId)))
                    ||
                    (m.ConversationId != null && _context.ConversationMembers.Any(cm => cm.ConversationId == m.ConversationId && cm.UserId == userId))
                )
                .CountAsync();

            var onlineFriends = await _context.Users
                .Where(u => friendIds.Contains(u.UserId) && u.Status == 1)
                .CountAsync();

            var activeServers = myServerIds.Count;

            return new DashboardSummaryDto
            {
                TotalMessages = totalMessages,
                OnlineFriends = onlineFriends,
                ActiveServers = activeServers
            };
        }

        public async Task<List<MyServerDto>> GetMyServersAsync(int userId)
        {
            var servers = await (
                from sm in _context.ServerMembers
                join s in _context.Servers on sm.ServerId equals s.ServerId
                where sm.UserId == userId
                orderby s.ServerName
                select new MyServerDto
                {
                    ServerId = s.ServerId,
                    ServerName = s.ServerName ?? "",
                    IconUrl = s.IconUrl,
                    ShortName = BuildShortName(s.ServerName)
                }
            ).ToListAsync();

            return servers;
        }

        public async Task<List<RecentServerDto>> GetRecentServersAsync(int userId)
        {
            var myServers = await (
                from sm in _context.ServerMembers
                join s in _context.Servers on sm.ServerId equals s.ServerId
                where sm.UserId == userId
                orderby sm.JoinedAt descending
                select new
                {
                    s.ServerId,
                    s.ServerName,
                    s.IconUrl,
                    s.TotalMembers
                }
            )
            .Take(6)
            .ToListAsync();

            var result = new List<RecentServerDto>();

            foreach (var server in myServers)
            {
                var channelIds = await _context.Channels
                    .Where(c => c.ServerId == server.ServerId)
                    .Select(c => c.ChannelId)
                    .ToListAsync();

                var lastMessage = await _context.Messages
                    .Where(m => m.ChannelId != null && channelIds.Contains(m.ChannelId.Value) && m.IsDeleted == 0)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefaultAsync();

                result.Add(new RecentServerDto
                {
                    ServerId = server.ServerId,
                    ServerName = server.ServerName ?? "",
                    IconUrl = server.IconUrl,
                    TotalMembers = server.TotalMembers,
                    LastMessagePreview = SafeDecrypt(lastMessage)
                });
            }

            return result;
        }

        public async Task<List<DirectMessageDto>> GetDirectMessagesAsync(int userId)
        {
            var conversations = await _context.ConversationMembers
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.ConversationId)
                .ToListAsync();

            var directConversationIds = await _context.Conversations
                .Where(c => conversations.Contains(c.ConversationId) && c.Type == 1)
                .Select(c => c.ConversationId)
                .ToListAsync();

            var result = new List<DirectMessageDto>();

            foreach (var conversationId in directConversationIds)
            {
                var otherUser = await (
                    from cm in _context.ConversationMembers
                    join u in _context.Users on cm.UserId equals u.UserId
                    where cm.ConversationId == conversationId && cm.UserId != userId
                    select u
                ).FirstOrDefaultAsync();

                if (otherUser == null) continue;

                var lastMessageEntity = await _context.Messages
                    .Where(m => m.ConversationId == conversationId && m.IsDeleted == 0)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefaultAsync();

                result.Add(new DirectMessageDto
                {
                    ConversationId = conversationId,
                    UserId = otherUser.UserId,
                    DisplayName = otherUser.DisplayName ?? otherUser.Username ?? otherUser.Email,
                    AvatarUrl = otherUser.AvatarUrl,
                    LastMessage = lastMessageEntity != null ? SafeDecrypt(lastMessageEntity.Content) : "",
                    LastMessageTime = lastMessageEntity?.CreatedAt,
                    UnreadCount = 0,
                    IsOnline = otherUser.Status == 1
                });
            }

            return result
                .OrderByDescending(x => x.LastMessageTime)
                .ToList();
        }

        public async Task<CurrentUserDto> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("Không tìm thấy user.");

            return new CurrentUserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                Status = user.Status
            };
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && n.IsRead == 0)
                .CountAsync();
        }

        private static string BuildShortName(string? serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                return "SV";

            var parts = serverName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(x => char.ToUpper(x[0]).ToString());

            var shortName = string.Join("", parts);

            return string.IsNullOrWhiteSpace(shortName) ? "SV" : shortName;
        }

        private string SafeDecrypt(string? cipher)
        {
            if (string.IsNullOrWhiteSpace(cipher))
                return "";

            try
            {
                return CryptoHelper.Decrypt(cipher);
            }
            catch
            {
                return cipher;
            }
        }
    }
}