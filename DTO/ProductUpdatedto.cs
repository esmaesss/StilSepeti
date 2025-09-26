using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    public class ProductUpdatedto
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; } = null!;
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
    }
}
