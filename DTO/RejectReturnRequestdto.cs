using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.DTO
{
    public class RejectReturnRequestdto
    {
        [Required(ErrorMessage = "İade ID zorunludur.")]
        public int ReturnId { get; set; }

        [Required(ErrorMessage = "Red sebebi zorunludur.")]
        [StringLength(500, ErrorMessage = "Red sebebi maksimum 500 karakter olabilir.")]
        public string Reason { get; set; } = string.Empty;
    }
}
