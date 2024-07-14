using TaskManagerAPI.Data.Repositories;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Services.Mappers;

namespace TaskManagerAPI.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetRolesAsync();
    }

    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Select(r => RoleMapper.MapToRoleDTO(r));
        }

    }
}
