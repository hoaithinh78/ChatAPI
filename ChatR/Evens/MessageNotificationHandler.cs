using ChatR.Interface;
using ChatR.Models;

namespace ChatR.Evens
{
    public class MessageNotificationHandler : IEventHandler<MessageCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public MessageNotificationHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task HandleAsync(MessageCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // Demo đơn giản: tạo notification cho conversation/channel
            var notification = new Notification
            {
                Type = 1,
                Content = $"New message from user {domainEvent.SenderId}",
                UserId = domainEvent.SenderId, // chỗ này thực tế phải là recipient
                CreatedAt = DateTime.UtcNow,
                IsRead = 0
            };

            await _notificationRepository.AddAsync(notification);
        }
    }
}
