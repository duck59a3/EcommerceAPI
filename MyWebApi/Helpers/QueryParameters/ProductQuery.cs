namespace MyWebApi.Helpers.QueryParameters
{
    public class ProductQuery : BaseQuery
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string? Category { get; set; }
        public int? CategoryId { get; set; }
        public bool? InStock { get; set; }
    }
}
