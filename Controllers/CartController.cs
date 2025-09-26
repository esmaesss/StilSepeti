using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    
    public class AddToCartDto
    {
        [Required(ErrorMessage = "Miktar belirtilmelidir.")]
        [Range(1, 100, ErrorMessage = "Miktar 1 ile 100 arasında olmalıdır.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Beden seçimi zorunludur.")]
        [RegularExpression("^(XS|S|M|L|XL)$", ErrorMessage = "Geçerli bedenler: XS, S, M, L, XL")]
        public string Size { get; set; } = null!;
    }

   
    public class UpdateCartQuantityDto
    {
        [Required(ErrorMessage = "Miktar belirtilmelidir.")]
        [Range(1, 100, ErrorMessage = "Miktar 1 ile 100 arasında olmalıdır.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Beden bilgisi gereklidir.")]
        public string Size { get; set; } = null!;
    }
}