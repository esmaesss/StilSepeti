using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Models;
using StilSepetiApp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace StilSepetiApp.Services
{
    public class UpdateProductService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UpdateProductService> _logger;

        public UpdateProductService(AppDbContext context, ILogger<UpdateProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> UpdateProductAsync(int productId, ProductUpdatedto dto, int sellerId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                _logger.LogWarning("Ürün bulunamadı: ProductId={ProductId}", productId);
                return ServiceResult.FailureBuilder("Ürün bulunamadı.");
            }

            if (product.SellerId != sellerId)
            {
                _logger.LogWarning("Yetkisiz güncelleme girişimi: ProductId={ProductId}, SellerId={SellerId}", productId, sellerId);
                return ServiceResult.FailureBuilder("Bu ürünü güncelleme yetkiniz yok.");
            }

          
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult.FailureBuilder("Ürün adı boş olamaz.");
            }

            if (dto.Price <= 0)
            {
                return ServiceResult.FailureBuilder("Fiyat pozitif olmalıdır.");
            }

            if (dto.Stock < 0)
            {
                return ServiceResult.FailureBuilder("Stok miktarı negatif olamaz.");
            }

       
            product.Name = dto.Name.Trim();
            product.Brand = dto.Brand?.Trim();
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl?.Trim();
            product.Category = dto.Category?.Trim();
            product.SubCategory = dto.SubCategory?.Trim();
            product.Size = dto.Size?.Trim();
            product.Stock = dto.Stock;

            
            product.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Ürün güncellendi: ProductId={ProductId}, SellerId={SellerId}, Name={ProductName}",
                productId, sellerId, product.Name);

            return ServiceResult.SuccessBuilder("Ürün başarıyla güncellendi.");
        }
    }
}