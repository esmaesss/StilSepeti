using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
using StilSepetiApp.Models;

namespace StilSepetiApp.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, IMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Productdto> CreateProductAsync(ProductCreatedto dto)
        {
            _logger.LogInformation("Yeni ürün oluşturuluyor: {ProductName}", dto.Name);

            // Validasyon
            if (dto.Stock < 0)
            {
                _logger.LogWarning("Negatif stok ile ürün oluşturulmaya çalışıldı: {ProductName}, Stock: {Stock}",
                    dto.Name, dto.Stock);
                throw new ArgumentException("Stok miktarı negatif olamaz.");
            }

            if (dto.Price <= 0)
            {
                _logger.LogWarning("Geçersiz fiyat ile ürün oluşturulmaya çalışıldı: {ProductName}, Price: {Price}",
                    dto.Name, dto.Price);
                throw new ArgumentException("Fiyat pozitif olmalıdır.");
            }

            // Seller'ın var olduğunu kontrol et
            var sellerExists = await _context.Users.AnyAsync(u => u.userId == dto.SellerId && u.Role == Role.Seller);
            if (!sellerExists)
            {
                _logger.LogWarning("Geçersiz seller ID ile ürün oluşturulmaya çalışıldı: SellerId={SellerId}", dto.SellerId);
                throw new ArgumentException("Geçersiz satıcı bilgisi.");
            }

            var product = _mapper.Map<Product>(dto);
            product.CreatedAt = DateTime.UtcNow;
            product.LastUpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ürün başarıyla oluşturuldu. ID: {ProductId}, Name: {ProductName}",
                product.Id, product.Name);

            return _mapper.Map<Productdto>(product);
        }

        public async Task<Productdto?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Ürün detayı getiriliyor: ProductId={ProductId}", id);

            var product = await _context.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                _logger.LogWarning("Ürün bulunamadı: ProductId={ProductId}", id);
                return null;
            }

            return _mapper.Map<Productdto>(product);
        }

        public async Task<PagedResult<Productdto>> GetProductsAsync(ProductFilter filter)
        {
            _logger.LogInformation("Ürünler filtreleniyor: {@Filter}", filter);

            var query = _context.Products.AsQueryable();

            // Filtreleme
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(p => p.Name.Contains(filter.Keyword) ||
                                        p.Description.Contains(filter.Keyword) ||
                                        p.Brand.Contains(filter.Keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.Brand))
                query = query.Where(p => p.Brand == filter.Brand);

            if (!string.IsNullOrWhiteSpace(filter.Size))
                query = query.Where(p => p.Size == filter.Size);

            if (!string.IsNullOrWhiteSpace(filter.Category))
                query = query.Where(p => p.Category == filter.Category);

            if (!string.IsNullOrWhiteSpace(filter.SubCategory))
                query = query.Where(p => p.SubCategory == filter.SubCategory);

            if (!string.IsNullOrWhiteSpace(filter.Colour))
                query = query.Where(p => p.Colour == filter.Colour);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            // Sadece stokta olan ürünleri göster
            query = query.Where(p => p.Stock > 0);

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderByDescending(p => p.CreatedAt) // En yeni ürünler en üstte
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoList = _mapper.Map<List<Productdto>>(products);

            _logger.LogInformation("{Count} ürün bulundu, {Page}. sayfa gösteriliyor", totalCount, filter.PageNumber);

            return new PagedResult<Productdto>
            {
                Data = dtoList,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ServiceResult> UpdateProductAsync(int id, ProductUpdatedto dto, int sellerId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId);

            if (product == null)
            {
                _logger.LogWarning("Ürün bulunamadı veya yetki yok: ProductId={ProductId}, SellerId={SellerId}", id, sellerId);
                return ServiceResult.FailureBuilder("Ürün bulunamadı veya güncelleme yetkiniz yok.");
            }

            // Validasyonlar
            if (dto.Stock < 0)
            {
                return ServiceResult.FailureBuilder("Stok miktarı negatif olamaz.");
            }

            if (dto.Price <= 0)
            {
                return ServiceResult.FailureBuilder("Fiyat pozitif olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult.FailureBuilder("Ürün adı boş olamaz.");
            }

            // Güncelleme
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
                id, sellerId, product.Name);

            return ServiceResult.SuccessBuilder("Ürün başarıyla güncellendi.");
        }

        public async Task<ServiceResult> DeleteProductAsync(int id, int sellerId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId);

            if (product == null)
            {
                _logger.LogWarning("Ürün bulunamadı veya yetki yok: ProductId={ProductId}, SellerId={SellerId}", id, sellerId);
                return ServiceResult.FailureBuilder("Ürün bulunamadı veya silme yetkiniz yok.");
            }

            // Aktif siparişlerde kullanılıp kullanılmadığını kontrol et
            var hasActiveOrders = await _context.OrderItems
                .AnyAsync(oi => oi.ProductId == id &&
                               (oi.Order!.Status == OrderStatus.Pending ||
                                oi.Order.Status == OrderStatus.Shipped));

            if (hasActiveOrders)
            {
                _logger.LogWarning("Aktif siparişlerde bulunan ürün silinmeye çalışıldı: ProductId={ProductId}", id);
                return ServiceResult.FailureBuilder("Aktif siparişlerde bulunan ürün silinemez.");
            }

            // Favorilerde bulunan ürünü temizle
            var favourites = await _context.Favourites
                .Where(f => f.ProductId == id)
                .ToListAsync();

            if (favourites.Any())
            {
                _context.Favourites.RemoveRange(favourites);
                _logger.LogInformation("Ürün favorilerden kaldırıldı: ProductId={ProductId}, FavouriteCount={Count}",
                    id, favourites.Count);
            }

            // Sepetteki ürünleri temizle
            var cartItems = await _context.CartItems
                .Where(c => c.ProductId == id)
                .ToListAsync();

            if (cartItems.Any())
            {
                _context.CartItems.RemoveRange(cartItems);
                _logger.LogInformation("Ürün sepetlerden kaldırıldı: ProductId={ProductId}, CartItemCount={Count}",
                    id, cartItems.Count);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ürün silindi: ProductId={ProductId}, SellerId={SellerId}, Name={ProductName}",
                id, sellerId, product.Name);

            return ServiceResult.SuccessBuilder("Ürün başarıyla silindi.");
        }

        public async Task<PagedResult<Productdto>> GetSellerProductsAsync(int sellerId, int pageNumber = 1, int pageSize = 20)
        {
            _logger.LogInformation("Satıcı ürünleri listeleniyor: SellerId={SellerId}", sellerId);

            // Seller'ın varlığını kontrol et
            var sellerExists = await _context.Users.AnyAsync(u => u.userId == sellerId && u.Role == Role.Seller);
            if (!sellerExists)
            {
                _logger.LogWarning("Geçersiz seller ID: SellerId={SellerId}", sellerId);
                return new PagedResult<Productdto>
                {
                    Data = new List<Productdto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var query = _context.Products.Where(p => p.SellerId == sellerId);
            var totalCount = await query.CountAsync();

            var products = await query
                .OrderByDescending(p => p.CreatedAt) // En yeni ürünler en üstte (gereksinim)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = _mapper.Map<List<Productdto>>(products);

            _logger.LogInformation("Satıcı ürünleri listelendi: SellerId={SellerId}, TotalCount={TotalCount}",
                sellerId, totalCount);

            return new PagedResult<Productdto>
            {
                Data = dtoList,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Ek yardımcı metodlar
        public async Task<List<string>> GetBrandsAsync()
        {
            _logger.LogInformation("Marka listesi getiriliyor");

            return await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.Brand))
                .Select(p => p.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            _logger.LogInformation("Kategori listesi getiriliyor");

            return await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.Category))
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<ServiceResult> UpdateStockAsync(int productId, int newStock, int sellerId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == sellerId);

            if (product == null)
            {
                return ServiceResult.FailureBuilder("Ürün bulunamadı veya yetkiniz yok.");
            }

            if (newStock < 0)
            {
                return ServiceResult.FailureBuilder("Stok miktarı negatif olamaz.");
            }

            var oldStock = product.Stock;
            product.Stock = newStock;
            product.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Ürün stoğu güncellendi: ProductId={ProductId}, OldStock={OldStock}, NewStock={NewStock}",
                productId, oldStock, newStock);

            return ServiceResult.SuccessBuilder($"Stok güncellendi: {oldStock} → {newStock}");
        }
    }
}