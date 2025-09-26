using System;
using System.Threading.Tasks;
using StilSepetiApp.Data;
using StilSepetiApp.Enums;
using StilSepetiApp.Services;


namespace StilSepetiApp.DTO
{
    public class UpdateOrderStatusService
    {
        private readonly AppDbContext _context;

        public UpdateOrderStatusService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return ServiceResult.FailureBuilder("Sipariþ bulunamadý.");

            if (order.Status == OrderStatus.Delivered && newStatus == OrderStatus.Cancelled)
                return ServiceResult.FailureBuilder("Teslim edilen sipariþ iptal edilemez.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ServiceResult.SuccessBuilder("Sipariþ durumu baþarýyla güncellendi.");
        }
    }
}