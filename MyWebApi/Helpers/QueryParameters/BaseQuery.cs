namespace MyWebApi.Helpers.QueryParameters
{
    public class BaseQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc"; // asc hoặc desc
        public string? Search { get; set; }
    }
}
