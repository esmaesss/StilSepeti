using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    public class Productdto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Görsel URL zorunludur.")]
        [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Kategori bilgisi zorunludur.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Alt kategori bilgisi zorunludur.")]
        public string SubCategory { get; set; }

        [Required(ErrorMessage = "Stok bilgisi zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Fiyat bilgisi zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat pozitif olmalıdır.")]
        public decimal Price { get; set; }

        public string? Colour { get; set; }

        [Required(ErrorMessage = "Marka bilgisi zorunludur.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Beden bilgisi zorunludur.")]
        public string Size { get; set; }
        [Required(ErrorMessage = "Satıcı ID zorunludur.")]
        public int SellerId { get; set; }

    }
}