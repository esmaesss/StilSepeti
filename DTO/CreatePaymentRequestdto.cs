
using System.ComponentModel.DataAnnotations;
using StilSepetiApp.Enums;

namespace StilSepetiApp.DTO
{
    public class CreatePaymentRequestdto
    {
        
            [Required(ErrorMessage = "Sipariş ID zorunludur.")]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
            public PaymentMethod Method { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "Geçerli bir tutar giriniz.")]
            public decimal Amount { get; set; }

            [StringLength(16, MinimumLength = 16, ErrorMessage = "Kart numarası 16 haneli olmalıdır.")]
            public string? CardNumber { get; set; }

            [StringLength(5, ErrorMessage = "Geçerli bir son kullanma tarihi giriniz.")]
            public string? ExpiryDate { get; set; }
        [StringLength(3, ErrorMessage = "Geçerli bir CVV giriniz.")]
        public string? CVV { get; set; }

        [StringLength(100, ErrorMessage = "Kart sahibi ismi en fazla 100 karakter olabilir.")]
        public string? CardHolderName { get; set; }
    }
}

