using ChatR.Models;

namespace ChatR.Interface
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdAsync(long notificationId);
        Task<List<Notification>> GetByUserIdAsync(long userId);
        Task<List<Notification>> GetUnreadByUserIdAsync(long userId);
        Task MarkAsReadAsync(long notificationId);
        Task MarkAllAsReadAsync(long userId);
        Task DeleteAsync(long notificationId);
    }
}
