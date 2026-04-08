using ChatR.Data;
using ChatR.DTOs.Friends;
using ChatR.Helpers;
using ChatR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Services.Friend
{
    public class FriendSidebarService : IFriendSidebarService
    {
        private readonly AppDbContext _context;

        public FriendSidebarService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FriendListItemDto>> GetMyFriendsAsync(int userId)
        {
            var friendIds1 = await _context.Friends
                .Where(f => f.UserId == userId)
                .Select(f => f.FriendId)
                .ToListAsync();

            var friendIds2 = await _context.Friends
                .Where(f => f.FriendId == userId)
                .Select(f => f.UserId)
                .ToListAsync();

            var friendIds = friendIds1.Union(friendIds2).Distinct().ToList();

            return await _context.Users
                .Where(u => friendIds.Contains(u.UserId))
                .OrderBy(u => u.DisplayName ?? u.Username)
                .Select(u => new FriendListItemDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    DisplayName = u.DisplayName,
                    AvatarUrl = u.AvatarUrl,
                    IsOnline = u.Status == 1
                })
                .ToListAsync();
        }

        public async Task<List<FriendListItemDto>> GetOnlineFriendsAsync(int userId)
        {
            var friends = await GetMyFriendsAsync(userId);
            return friends.Where(x => x.IsOnline).ToList();
        }

        public async Task<List<FriendSidebarItemDto>> GetFriendSidebarAsync(int userId)
        {
            var friends = await GetMyFriendsAsync(userId);
            var result = new List<FriendSidebarItemDto>();

            foreach (var friend in friends)
            {
                var conversationId = await _context.ConversationMembers
                    .Where(cm => cm.UserId == userId)
                    .Select(cm => cm.ConversationId)
                    .Where(cid => _context.Conversations.Any(c => c.ConversationId == cid && c.Type == 1))
                    .Where(cid => _context.ConversationMembers.Any(cm => cm.ConversationId == cid && cm.UserId == friend.UserId))
                    .FirstOrDefaultAsync();

                string lastMessage = "";
                DateTime? lastMessageTime = null;

                if (conversationId > 0)
                {
                    var lastMsg = await _context.Messages
                        .Where(m => m.ConversationId == conversationId && m.IsDeleted == 0)
                        .OrderByDescending(m => m.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (lastMsg != null)
                    {
                        lastMessageTime = lastMsg.CreatedAt;
                        lastMessage = SafeDecrypt(lastMsg.Content);
                    }
                }

                result.Add(new FriendSidebarItemDto
                {
                    UserId = friend.UserId,
                    ConversationId = conversationId > 0 ? conversationId : null,
                    Username = friend.Username,
                    DisplayName = friend.DisplayName,
                    AvatarUrl = friend.AvatarUrl,
                    IsOnline = friend.IsOnline,
                    LastMessage = lastMessage,
                    LastMessageTime = lastMessageTime,
                    UnreadCount = 0
                });
            }

            return result
                .OrderByDescending(x => x.LastMessageTime)
                .ThenBy(x => x.DisplayName)
                .ToList();
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
                return cipher ?? "";
            }
        }
    }
}