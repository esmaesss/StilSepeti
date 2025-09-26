using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
using StilSepetiApp.Models;

namespace StilSepetiApp.Services
{
    public class ReturnService : IReturnService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReturnService> _logger;

        public ReturnService(AppDbContext context, ILogger<ReturnService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ReturnRequestdto>> GetAllAsync()
        {
            _logger.LogInformation("Tüm iade talepleri listeleniyor");
            var requests = await _context.ReturnRequests
           .Include(r => r.Order)
           .ToListAsync();

            _logger.LogInformation("Toplam {Count} iade talebi listelendi", requests.Count);

            
            return requests.Select(r => new ReturnRequestdto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                Reason = r.Reason,
                Status = r.Status,
                CreatedAt = r.RequestedAt,
            }).ToList();
        }

        public async Task<ServiceResult> UpdateStatusAsync(int id, ReturnStatus newStatus)
        {
            _logger.LogInformation("İade talebi durumu güncelleniyor: TalepId={ReturnId}, YeniDurum={NewStatus}", id, newStatus);
            var request = await _context.ReturnRequests.FindAsync(id);
            if (request == null)
            {
                _logger.LogWarning("İade talebi bulunamadı: TalepId={ReturnId}", id);
                return ServiceResult.FailureBuilder("İade talebi bulunamadı.");
            }

            if (request.Status != ReturnStatus.Requested)
            {
                _logger.LogWarning("İade talebi zaten değerlendirilmiş: TalepId={ReturnId}", id);
                return ServiceResult.FailureBuilder("Talep zaten değerlendirilmiş.");
            }

            request.Status = newStatus;
            request.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("İade talebi güncellendi: TalepId={ReturnId}, YeniDurum={NewStatus}", id, newStatus);

            return ServiceResult.SuccessBuilder("İade talebi güncellendi.");
        }
    }
}