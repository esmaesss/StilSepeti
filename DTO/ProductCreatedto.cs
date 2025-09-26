using System.ComponentModel.DataAnnotations;
namespace StilSepetiApp.DTO
{
    public class ProductCreatedto
    {
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }


        [Required(ErrorMessage = "Kategori bilgisi zorunludur.")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "Alt kategori bilgisi zorunludur.")]
        public string SubCategory { get; set; } = null!;

        [Required(ErrorMessage = "Stok bilgisi zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Fiyat bilgisi zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat pozitif olmalıdır.")]
        public decimal Price { get; set; }

        public string? Colour { get; set; }

        [Required(ErrorMessage = "Marka bilgisi zorunludur.")]
        public string Brand { get; set; } = null!;

        [Required(ErrorMessage = "Beden bilgisi zorunludur.")]
        public string Size { get; set; } = null!;

        public int SellerId { get; set; }
        [Required(ErrorMessage = "Ürün görseli zorunludur.")]
        public IFormFile ImageFile { get; set; } = null!;
    }
}