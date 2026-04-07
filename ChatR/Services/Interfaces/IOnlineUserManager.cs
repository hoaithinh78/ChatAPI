namespace ChatR.Services.Interfaces
{
    public interface IOnlineUserManager
    {
        void AddConnection(int userId, string connectionId);
        void RemoveConnection(string connectionId);
        bool IsUserOnline(int userId);
        List<string> GetConnections(int userId);
        List<int> GetOnlineUsers();
    }
}
