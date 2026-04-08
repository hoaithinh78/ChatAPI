using ChatR.Interface.Observer;
using ChatR.Interface.Repository;
using ChatR.Models;

namespace ChatR.Evens
{
    public class MessageNotificationHandler : IEventHandler<MessageCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository; 

        public MessageNotificationHandler(
            INotificationRepository notificationRepository,
            IUserRepository userRepository)  
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task HandleAsync(MessageCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            if (domainEvent.ConversationId == null) return;

            var members = await _userRepository.GetUsersInConversation(domainEvent.ConversationId.Value);

            foreach (var member in members.Where(m => m.UserId != domainEvent.SenderId))
            {
                var notification = new Notification
                {
                    Type = 1,
                    Content = $"New message from user {domainEvent.SenderId}",
                    UserId = member.UserId, // recipient thực sự
                    CreatedAt = DateTime.UtcNow,
                    IsRead = 0
                };

                await _notificationRepository.AddAsync(notification);
            }
        }
    }
}