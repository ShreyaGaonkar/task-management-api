namespace TaskManagerAPI.DTO.Request
{
    public class TaskRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string SortField { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "Desc";
    }
}

