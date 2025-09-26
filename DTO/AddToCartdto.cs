using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    public class AddToCartdto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string Size { get; set; } = null!;
    }
}
