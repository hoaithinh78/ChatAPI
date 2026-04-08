using ChatR.Data;
using ChatR.Interface.Repository;
using ChatR.Models;
using Microsoft.EntityFrameworkCore;
namespace ChatR.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersInConversation(int conversationId)
        {
            return await _context.ConversationMembers
                .Where(cm => cm.ConversationId == conversationId)
                .Select(cm => cm.User!)
                .ToListAsync();
        }
    }
}
