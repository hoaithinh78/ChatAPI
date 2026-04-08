using System.Collections.Concurrent;

namespace ChatR.Interface.Singleton
{
    public class OnlineUserTracker : IOnlineUserTracker
    {
        private readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();
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
            lock (_lock)
            {
                return _userConnections.TryGetValue(userId, out var connections) && connections.Count > 0;
            }
        }

        public int GetConnectionCount(int userId)
        {
            lock (_lock)
            {
                return _userConnections.TryGetValue(userId, out var connections)
                    ? connections.Count
                    : 0;
            }
        }

        public List<string> GetConnections(int userId)
        {
            lock (_lock)
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    return connections.ToList();
                }

                return new List<string>();
            }
        }

        public int GetOnlineUsersCount()
        {
            lock (_lock)
            {
                return _userConnections.Count;
            }
        }

        public List<int> GetOnlineUserIds()
        {
            lock (_lock)
            {
                return _userConnections.Keys.ToList();
            }
        }
    }

}
