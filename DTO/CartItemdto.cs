using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    public class CartItemDto
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        public int ProductId { get; set; }

        public string? ProductName { get; set; } 

        [Required(ErrorMessage = "Adet bilgisi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalıdır.")]
        public int Quantity { get; set; }

        public decimal? UnitPrice { get; set; } 
        public string? ImageUrl { get; set; }
        public string Size { get; set; }  // XS, S, M, L, XL

    }
}