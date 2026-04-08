using ChatR.Models;

namespace ChatR.Interface.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersInConversation(int conversationId);
    }
}
