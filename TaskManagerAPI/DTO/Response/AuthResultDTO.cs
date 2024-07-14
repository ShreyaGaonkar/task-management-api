namespace TaskManagerAPI.DTO.Response
{
    public class AuthResultDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
