using System.Threading;

namespace newltweb.Services
{
    public static class GlobalCurrentUser
    {
        private static string? _username;
        private static string? _id;

        // Use a simple lock for thread-safety
        private static readonly object _lock = new();

        public static void Set(string username, string? id)
        {
            if (username == null) return;
            lock (_lock)
            {
                _username = username;
                _id = id;
            }
        }

        public static (string? Username, string? Id) Get()
        {
            lock (_lock)
            {
                return (_username, _id);
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _username = null;
                _id = null;
            }
        }
    }
}
