using StilSepetiApp.Enums;
namespace StilSepetiApp.DTO
{
    public class PaymentResponsedto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TransactionId { get; set; }
        public bool RequiresAction { get; set; } 
        public string? ActionUrl { get; set; }
    }
}
