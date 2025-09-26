using System.ComponentModel.DataAnnotations;
using StilSepetiApp.Enums;

namespace StilSepetiApp.DTO
{
    public class UpdateReturnStatusdto
    {

        [Required(ErrorMessage = "Yeni durum zorunludur.")]
        public ReturnStatus NewStatus { get; set; }
}
}


