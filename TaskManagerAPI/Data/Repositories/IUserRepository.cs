using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> UserExistByEmailAsync(string email);
        Task<User> GetUserWithRolesAsync(string email);
        Task<User> GetUserWithRolesAsync(int userId);
    }
}
