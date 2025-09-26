using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Models;
using Microsoft.EntityFrameworkCore;


namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Seller")]
    public class SellerDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SellerDashboardController(AppDbContext context, IMapper mapper)
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
            return NotFound("Henüz ürün eklemediniz.");

            var dtoList = _mapper.Map<List<Productdto>>(products);
            return Ok(dtoList);
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
            if (!orders.Any())
                return NotFound("Henüz sipariş almadınız.");

            var mapped = orders.Select(o => new
            {
                o.Id,
                o.Status,
                o.CreatedAt,
                TotalItems = o.Items.Count,
                TotalAmount = o.StoredTotalAmount
            });

            return Ok(mapped);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetSellerStats()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var products = await _context.Products
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Items.Any(oi => oi.Product.SellerId == sellerId))
                .ToListAsync();

            if (!products.Any() && !orders.Any())
                return NotFound("Henüz ürün eklemediniz veya sipariş almadınız.");

            var topSelling = orders
                .SelectMany(o => o.Items)
                .Where(i => i.Product.SellerId == sellerId)
                .GroupBy(i => i.Product.Name)
                .Select(g => new { Product = g.Key, TotalSold = g.Sum(i => i.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(5);

            var lowStock = products
                .Where(p => p.Stock < 5)
                .Select(p => new { p.Name, p.Stock });

            return Ok(new
            {
                TotalProducts = products.Count,
                TotalOrders = orders.Count,
                TopSellingProducts = topSelling,
                LowStockWarnings = lowStock
            });
        }
    }

}
