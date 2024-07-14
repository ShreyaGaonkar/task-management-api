using TaskManagerAPI.Data.Repositories;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Exceptions;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Mappers;

namespace TaskManagerAPI.Services
{
    public interface ITeamService
    {
        Task<TeamDTO> CreateTeamAsync(TeamCreateDTO teamCreateDTO);
        Task<TeamDTO> UpdateTeamAsync(int id, TeamCreateDTO teamCreateDTO);
        Task<TeamDTO> GetTeamByIdAsync(int id);
        Task<PaginatedList<TeamDTO>> GetTeamsAsync(TeamRequestDTO teamRequestDTO);
    }

    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IRepository<UserTeam> _userTeamRepository;
        private readonly IUserContextService _userContextService;

        public TeamService(ITeamRepository teamRepository, IRepository<UserTeam> userTeamRepository, IUserContextService userContextService)
        {
            _teamRepository = teamRepository;
            _userTeamRepository = userTeamRepository;
            _userContextService = userContextService;
        }

        public async Task<TeamDTO> CreateTeamAsync(TeamCreateDTO teamCreateDTO)
        {
            var teamExists = await _teamRepository.GetTeamByName(teamCreateDTO.TeamName);
            if (teamExists != null)
            {
                throw new NotFoundException(Captions.TeamNameAlreadyExist);
            }

            var teamDTO = new Team
            {
                TeamName = teamCreateDTO.TeamName,
                CreatedBy = _userContextService.UserId,
                CreatedDate = DateTime.UtcNow
            };

            var NewTeam = await _teamRepository.InsertAsync(teamDTO);

            if (teamCreateDTO.UserIds != null && teamCreateDTO.UserIds.Any())
            {
                var teamMember = teamCreateDTO.UserIds.Select(userId => new UserTeam
                {
                    TeamId = NewTeam.Id,
                    UserId = userId,
                    AssignedDate = DateTime.UtcNow
                }).ToList();

                await _userTeamRepository.InsertRangeAsync(teamMember);
            }

            var team = await _teamRepository.GetTeamWithMembersAsync(NewTeam.Id);
            return TeamMapper.MapToTeamDTO(team);
        }

        public async Task<TeamDTO> UpdateTeamAsync(int id, TeamCreateDTO teamCreateDTO)
        {
            var teamExists = await _teamRepository.GetTeamByName(teamCreateDTO.TeamName);
            if (teamExists != null && teamExists.Id != id)
            {
                throw new NotFoundException(Captions.TeamNameAlreadyExist);
            }

            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
            {
                throw new NotFoundException(Captions.TeamNotFound);
            }

            existingTeam.TeamName = teamCreateDTO.TeamName;
            existingTeam.UpdatedBy = _userContextService.UserId;
            existingTeam.UpdatedDate = DateTime.UtcNow;

            var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);

            var existingTeamMembers = await _userTeamRepository.GetAllAsync(tp => tp.TeamId == id);
            var existingUserIds = existingTeamMembers.Select(tp => tp.UserId).ToList();
            var newUserIds = teamCreateDTO.UserIds;

            var userIdsToRemove = existingUserIds.Except(newUserIds).ToList();
            if (userIdsToRemove.Any())
            {
                var teamProjectsToRemove = existingTeamMembers.Where(tp => userIdsToRemove.Contains(tp.UserId)).ToList();
                await _userTeamRepository.DeleteRangeAsync(teamProjectsToRemove);
            }

            var userIdsToAdd = newUserIds.Except(existingUserIds).ToList();
            if (userIdsToAdd.Any())
            {
                var teamUsersToAdd = userIdsToAdd.Select(userId => new UserTeam
                {
                    TeamId = id,
                    UserId = userId,
                    AssignedDate = DateTime.UtcNow
                }).ToList();
                await _userTeamRepository.InsertRangeAsync(teamUsersToAdd);
            }

            var team = await _teamRepository.GetTeamWithMembersAsync(id);
            return TeamMapper.MapToTeamDTO(team);
        }

        public async Task<TeamDTO> GetTeamByIdAsync(int id)
        {
            var team = await _teamRepository.GetTeamWithMembersAsync(id);

            if (team == null)
            {
                throw new NotFoundException(Captions.TeamNotFound);
            }

            return TeamMapper.MapToTeamDTO(team);
        }

        public async Task<PaginatedList<TeamDTO>> GetTeamsAsync(TeamRequestDTO teamRequestDTO)
        {
            var paginatedProjects = await _teamRepository.GetPagedAsync(teamRequestDTO);

            var teamDTOs = paginatedProjects.Data.Select(TeamMapper.MapToTeamDTO).ToList();
            var paginatedDTO = new PaginatedList<TeamDTO>(teamDTOs, paginatedProjects.Total);

            return paginatedDTO;
        }
    }
}
