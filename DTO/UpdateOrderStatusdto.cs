using StilSepetiApp.Enums;

namespace StilSepetiApp.DTO
{
    public class UpdateOrderStatusdto
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
