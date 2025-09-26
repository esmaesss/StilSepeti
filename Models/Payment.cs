using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StilSepetiApp.Enums;

namespace StilSepetiApp.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        public string? TransactionId { get; set; } 

        [StringLength(1000)]
        public string? PaymentDetails { get; set; } 

        [StringLength(500)]
        public string? ErrorMessage { get; set; } 
    }
}