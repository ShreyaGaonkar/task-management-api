namespace TaskManagerAPI.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ServiceResult()
        {
            Success = true;
        }

        public ServiceResult(string message)
        {
            Success = false;
            Message = message;
        }
    }
}
