using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
using StilSepetiApp.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<ServiceResult> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
}