using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Mappers
{
    public static class RoleMapper
    {
        public static RoleDTO MapToRoleDTO(Role role)
        {
            return new RoleDTO
            {
                Id = role.Id,
                RoleName = role.RoleName
            };
        }
    }
}
