using StilSepetiApp.Enums;

namespace StilSepetiApp.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemdto> Items { get; set; }

    }
}
