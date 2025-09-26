using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    public class UpdateStockdto
    {
        [Required(ErrorMessage = "Yeni stok miktarı belirtilmelidir.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz.")]
        public int NewStock { get; set; }
    }
}