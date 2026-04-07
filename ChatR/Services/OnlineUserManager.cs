using ChatR.Services.Interfaces;
using System.Collections.Concurrent;

namespace ChatR.Services
{
    public class OnlineUserManager : IOnlineUserManager
    {
        // userId -> list connectionId
        private readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();

        // connectionId -> userId
        private readonly ConcurrentDictionary<string, int> _connectionUsers = new();

        private readonly object _lock = new();

        public void AddConnection(int userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = new HashSet<string>();
                }

                _userConnections[userId].Add(connectionId);
                _connectionUsers[connectionId] = userId;
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (_lock)
            {
                if (!_connectionUsers.TryGetValue(connectionId, out var userId))
                    return;

                _connectionUsers.TryRemove(connectionId, out _);

                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _userConnections.TryRemove(userId, out _);
                    }
                }
            }
        }

        public bool IsUserOnline(int userId)
        {
            return _userConnections.TryGetValue(userId, out var connections) && connections.Count > 0;
        }

        public List<string> GetConnections(int userId)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                lock (_lock)
                {
                    return connections.ToList();
                }
            }

            return new List<string>();
        }

        public List<int> GetOnlineUsers()
        {
            return _userConnections.Keys.ToList();
        }
    }
}
