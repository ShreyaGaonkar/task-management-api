namespace TaskManagerAPI.DTO.Response
{
    public class TaskDocumentResponseDTO
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime UploadedDate { get; set; }
        public string UploadedByUser { get; set; }
    }
}
