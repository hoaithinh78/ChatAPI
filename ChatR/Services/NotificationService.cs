using ChatR.Data;
using ChatR.DTOs.Common;
using ChatR.DTOs.Notification;
using ChatR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationListResponseDto> GetMyNotificationsAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationItemDto
                {
                    NotificationId = x.NotificationId,
                    Type = x.Type,
                    Content = x.Content,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var unreadCount = notifications.Count(x => x.IsRead == 0);

            return new NotificationListResponseDto
            {
                Items = notifications,
                TotalCount = notifications.Count,
                UnreadCount = unreadCount
            };
        }

        public async Task<UnreadCountDto> GetUnreadCountAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var unreadCount = await _context.Notifications
                .CountAsync(x => x.UserId == userId && x.IsRead == 0, cancellationToken);

            return new UnreadCountDto
            {
                UnreadCount = unreadCount
            };
        }

        public async Task<MessageResponseDto> MarkAsReadAsync(
            int userId,
            int notificationId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(
                    x => x.NotificationId == notificationId && x.UserId == userId,
                    cancellationToken);

            if (notification == null)
                throw new Exception("Thông báo không tồn tại.");

            if (notification.IsRead == 0)
            {
                notification.IsRead = 1;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new MessageResponseDto
            {
                Message = "Notification marked as read"
            };
        }

        public async Task<MessageResponseDto> MarkAllAsReadAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var unreadNotifications = await _context.Notifications
                .Where(x => x.UserId == userId && x.IsRead == 0)
                .ToListAsync(cancellationToken);

            if (unreadNotifications.Count > 0)
            {
                foreach (var item in unreadNotifications)
                {
                    item.IsRead = 1;
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            return new MessageResponseDto
            {
                Message = "All notifications marked as read"
            };
        }

        public async Task<MessageResponseDto> DeleteNotificationAsync(
            int userId,
            int notificationId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(
                    x => x.NotificationId == notificationId && x.UserId == userId,
                    cancellationToken);

            if (notification == null)
                throw new Exception("Thông báo không tồn tại.");

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return new MessageResponseDto
            {
                Message = "Notification deleted"
            };
        }
    }
}