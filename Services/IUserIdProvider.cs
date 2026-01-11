namespace newltweb.Services
{
    public interface IUserIdProvider
    {
        Task<string?> GetUsernameAsync();

        Task<string?> GetCustomerOrEmployeeIdAsync();
        Task RecordLoginAsync(string username, string? id);
        Task<(string? Username, string? Id)> GetCurrentLoginAsync();
    }
}
