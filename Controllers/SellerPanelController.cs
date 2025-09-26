using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using StilSepetiApp.Data;
using StilSepetiApp.Models;
using StilSepetiApp.DTO;
using AutoMapper;


namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Seller")]
    public class SellerPanelController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SellerPanelController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       
        [HttpGet("products")]
        public async Task<IActionResult> GetMyProducts()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var products = await _context.Products
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();

            var dtoList = _mapper.Map<List<Productdto>>(products);
            return Ok(dtoList);
        }

       
        [HttpPost("products")]
        public async Task<IActionResult> AddProduct([FromBody] Productdto dto)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            dto.SellerId = sellerId;

            string imagePath = null;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                imagePath = "/images/products/" + fileName;
            }

            var product = _mapper.Map<Product>(dto);
            product.ImageUrl = imagePath;
            product.CreatedAt = DateTime.UtcNow;
            product.LastUpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok("Ürün başarıyla eklendi.");

        }


        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Productdto dto)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId);

            if (product == null)
                return NotFound("Ürün bulunamadı veya yetkiniz yok.");

            _mapper.Map(dto, product);
            await _context.SaveChangesAsync();

            return Ok("Ürün güncellendi.");
        }

       
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId);

            if (product == null)
                return NotFound("Ürün bulunamadı veya yetkiniz yok.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Ürün silindi.");
        }

       
        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Items.Any(oi => oi.Product.SellerId == sellerId))
                .ToListAsync();

            return Ok(orders); 
        }
    }
}