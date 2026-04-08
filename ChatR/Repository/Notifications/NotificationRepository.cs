using ChatR.Data;
using ChatR.Models;
using Microsoft.EntityFrameworkCore;
using ChatR.Interface.Repository;

namespace ChatR.Repository.Notifications
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification?> GetByIdAsync(long notificationId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);
        }

        public async Task<List<Notification>> GetByUserIdAsync(long userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(long userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId && x.IsRead == 0)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(long notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);

            if (notification == null) return;

            notification.IsRead = 1;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(long userId)
        {
            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId && x.IsRead == 0)
                .ToListAsync();

            if (!notifications.Any()) return;

            foreach (var notification in notifications)
            {
                notification.IsRead = 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);

            if (notification == null) return;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}