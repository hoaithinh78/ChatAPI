using ChatR.Models;

namespace ChatR.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersInConversation(int conversationId);
    }
}
