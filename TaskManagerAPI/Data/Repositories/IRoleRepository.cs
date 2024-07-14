using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<bool> RoleExistsAsync(int roleId);
    }
}
