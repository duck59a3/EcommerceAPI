namespace MyWebApi.Responses
{
    public class PagedResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }

        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords) 
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            HasNextPage = pageNumber < TotalPages;
            HasPreviousPage = pageNumber > 1;

            //if (HasNextPage)
            //    NextPageUrl = $"{baseUrl}?pageNumber={pageNumber + 1}&pageSize={pageSize}";
            //if (HasPreviousPage)
            //    PreviousPageUrl = $"{baseUrl}?pageNumber={pageNumber - 1}&pageSize={pageSize}";
        }
    }
}
