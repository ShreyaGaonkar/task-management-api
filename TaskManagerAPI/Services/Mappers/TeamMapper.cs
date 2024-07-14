using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Mappers
{
    public static class TeamMapper
    {
        public static TeamDTO MapToTeamDTO(Team team)
        {
            return new TeamDTO
            {
                Id = team.Id,
                TeamName = team.TeamName,
                TeamMembers = team?.UserTeams?
                                .Where(ut => ut?.User != null)
                                .Select(ut => ut.User)
                                .Select(user => MapToUserDTO(user))
                                .ToList() ?? new List<UserDTO>()
            };
        }

        public static UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public static Team MapToTeam(Team team, TeamDTO teamDTO, int userId)
        {
            team.TeamName = teamDTO.TeamName;
            team.UpdatedBy = userId;
            team.UpdatedDate = DateTime.UtcNow;

            return team;
        }
    }
}
