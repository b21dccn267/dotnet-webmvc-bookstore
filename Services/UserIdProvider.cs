using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace newltweb.Services
{
    public class UserIdProvider : IUserIdProvider
    {
        private readonly ILogger<UserIdProvider> _logger;

        private readonly ConcurrentDictionary<string, string?> _lastSeenIds = new();

        // the "current" record for this provider instance
        private string? _currentUsername;
        private string? _currentId;

        public UserIdProvider(ILogger<UserIdProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Called during login to record mapping and mark this record as current
        public Task RecordLoginAsync(string username, string? id)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Task.CompletedTask;

            username = username.Trim();
            _lastSeenIds.AddOrUpdate(username, id, (_, __) => id);

            // mark as current for this provider instance
            _currentUsername = username;
            _currentId = id;

            _logger.LogInformation("Recorded login: {Username} -> {Id}", username, id ?? "(null)");
            return Task.CompletedTask;
        }

        // Returns the current username recorded in this provider instance (or null)
        public Task<string?> GetUsernameAsync()
        {
            return Task.FromResult<string?>(_currentUsername);
        }

        // Returns the current id (customer or employee) recorded in this provider instance (or null)
        public Task<string?> GetCustomerOrEmployeeIdAsync()
        {
            return Task.FromResult<string?>(_currentId);
        }

        // Returns tuple (username, id) for the current record only
        public Task<(string? Username, string? Id)> GetCurrentLoginAsync()
        {
            return Task.FromResult<(string?, string?)>((_currentUsername, _currentId));
        }
    }
}
