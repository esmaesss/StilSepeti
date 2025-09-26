using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;


namespace StilSepetiApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, IMapper mapper, ILogger<OrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            _logger.LogInformation("Toplam {Count} sipariş listelendi.", orders.Count);

            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<ServiceResult> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Sipariş bulunamadı: SiparişId={OrderId}", orderId);
                return ServiceResult.FailureBuilder("Sipariş bulunamadı.");
            }

            if (order.Status == OrderStatus.Delivered && newStatus == OrderStatus.Cancelled)
            {
                _logger.LogWarning("Teslim edilen sipariş iptal edilemez: SiparişId={OrderId}", orderId);
                return ServiceResult.FailureBuilder("Teslim edilen sipariş iptal edilemez.");
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Sipariş durumu güncellendi: SiparişId={OrderId}, YeniDurum={NewStatus}", orderId, newStatus);

            return ServiceResult.SuccessBuilder("Sipariş durumu başarıyla güncellendi.");
        }
    }
}