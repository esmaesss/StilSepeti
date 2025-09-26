namespace StilSepetiApp.DTO
{
    public class ProductFilter
    {
        public string? Keyword { get; set; }
        public string? Brand { get; set; }
        public string? Size { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? Colour { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}