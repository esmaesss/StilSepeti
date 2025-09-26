using System.Data;
using System.Text.Json.Serialization;
using StilSepetiApp.Enums;

namespace StilSepetiApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
        public User? User { get; set; }

        public List<OrderItem> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime UpdatedAt { get; set; }

       
        public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);

        
        public decimal StoredTotalAmount { get; set; }
        
        public string ShippingAddress { get; set; }

        public List<Payment> Payments { get; set; } = new();
       

    }
}