namespace TaskManagerAPI.DTO.Response
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public List<UserDTO> TeamMembers { get; set; }
    }
}
