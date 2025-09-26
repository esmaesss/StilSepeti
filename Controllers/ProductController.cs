using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Models;
using AutoMapper;
using StilSepetiApp.Services;
using Microsoft.Extensions.Logging;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(AppDbContext context, IMapper mapper, IProductService productService, ILogger<ProductController> logger)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<Productdto>>> GetProductsFiltered([FromQuery] ProductFilter filter)
        {
            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<Productdto>> CreateProduct([FromForm] ProductCreatedto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            dto.SellerId = sellerId;

            try
            {
                
                var fileName = $"{Guid.NewGuid()}_{dto.ImageFile.FileName}";
                var filePath = Path.Combine("wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/{fileName}";
                dto.ImageFile = imageUrl; 

                var resultDto = await _productService.CreateProductAsync(dto);
                _logger.LogInformation("Yeni ürün oluşturuldu: ProductId={ProductId}, SellerId={SellerId}", resultDto.Id, sellerId);
                return CreatedAtAction(nameof(GetProduct), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün oluşturma sırasında hata: SellerId={SellerId}", sellerId);
                return StatusCode(500, "Ürün oluşturulurken bir hata oluştu.");
            }
        }




        [HttpGet]
        public async Task<ActionResult<PagedResult<Productdto>>> GetAllProducts(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 20)
        {
           
            if (pageSize > 50) pageSize = 50;
            if (pageNumber < 1) pageNumber = 1;

            var filter = new ProductFilter
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? keyword,
            [FromQuery] string? brand,
            [FromQuery] string? size,
            [FromQuery] string? category,
            [FromQuery] string? subcategory,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var filter = new ProductFilter
            {
                Keyword = keyword,
                Brand = brand,
                Size = size,
                Category = category,
                SubCategory = subcategory,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Productdto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Ürün bulunamadı: ProductId={ProductId}", id);
                return NotFound("Ürün bulunamadı.");
            }

            return Ok(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<string>>> GetBrands()
        {
            var brands = await _productService.GetBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost("seed")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedProducts()
        {
            
            var adminExists = await _context.Users.AnyAsync(u => u.Role == StilSepetiApp.Enums.Role.Admin);
            if (!adminExists)
            {
                return BadRequest("Admin kullanıcısı bulunamadı. Önce admin kullanıcısı oluşturun.");
            }

            var adminUser = await _context.Users.FirstAsync(u => u.Role == StilSepetiApp.Enums.Role.Admin);

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Oversize T-Shirt",
                    Description = "Rahat kesim oversize t-shirt",
                    Price = 199.90m,
                    Stock = 50,
                    Brand = "Zara",
                    Size = "M",
                    Category = "Kadın",
                    SubCategory = "Üst Giyim",
                    ImageUrl = "https://example.com/tshirt.jpg",
                    Colour = "Beyaz",
                    SellerId = adminUser.userId,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Skinny Jean",
                    Description = "Dar kesim jean pantolon",
                    Price = 299.90m,
                    Stock = 30,
                    Brand = "H&M",
                    Size = "L",
                    Category = "Kadın",
                    SubCategory = "Alt Giyim",
                    ImageUrl = "https://example.com/jean.jpg",
                    Colour = "Mavi",
                    SellerId = adminUser.userId,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seed products eklendi: Count={Count}", products.Count);
            return Ok($"{products.Count} ürün başarıyla eklendi.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdatedto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _productService.UpdateProductAsync(id, dto, sellerId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _productService.DeleteProductAsync(id, sellerId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockdto dto)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _productService.UpdateStockAsync(id, dto.NewStock, sellerId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
    }
}