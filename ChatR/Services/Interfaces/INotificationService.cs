using ChatR.DTOs.Common;
using ChatR.DTOs.Notification;

namespace ChatR.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationListResponseDto> GetMyNotificationsAsync(int userId, CancellationToken cancellationToken = default);
        Task<UnreadCountDto> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);
        Task<MessageResponseDto> MarkAsReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default);
        Task<MessageResponseDto> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);
        Task<MessageResponseDto> DeleteNotificationAsync(int userId, int notificationId, CancellationToken cancellationToken = default);
    }
}
