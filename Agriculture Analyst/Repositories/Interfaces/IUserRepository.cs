using Agriculture_Analyst.Models;

namespace Agriculture_Analyst.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UserExistsAsync(string username, string email);
        Task<User> CreateUserAsync(User user);
        Task SaveChangesAsync();
    }
}
