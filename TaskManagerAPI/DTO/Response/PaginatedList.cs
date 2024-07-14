namespace TaskManagerAPI.DTO.Response
{
    public class PaginatedList<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }

        public PaginatedList(List<T> data, int total)
        {
            Data = data;
            Total = total;
        }
    }
}
