namespace ChatR.Interface.Singleton
{
    public interface IOnlineUserTracker
    {
        void AddConnection(int userId, string connectionId);
        void RemoveConnection(string connectionId);
        bool IsUserOnline(int userId);
        int GetConnectionCount(int userId);
        List<string> GetConnections(int userId);
        int GetOnlineUsersCount();
        List<int> GetOnlineUserIds();
    }
}
