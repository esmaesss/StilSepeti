using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.Enums;

namespace StilSepetiApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/analytics")]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminAnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
           
            var orderItems = await _context.OrderItems
                .Include(i => i.Product)
                .ToListAsync();

            var topSelling = orderItems
                .GroupBy(i => i.Product.Name)
                .Select(g => new { Product = g.Key, TotalSold = g.Sum(i => i.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToList();

          
            var favourites = await _context.Favourites
                .Include(f => f.Product)
                .ToListAsync();

            var topFavourites = favourites
                .GroupBy(f => f.Product.Name)
                .Select(g => new { Product = g.Key, FavouriteCount = g.Count() })
                .OrderByDescending(g => g.FavouriteCount)
                .Take(5)
                .ToList();

           
            var sellers = await _context.Users
                .Where(u => u.Role == Role.Seller)
                .ToListAsync();

            var sellerStats = sellers.Select(s => new
            {
                SellerName = s.Username,
                ProductCount = _context.Products.Count(p => p.SellerId == s.userId),
                TotalSales = orderItems.Where(i => i.Product.SellerId == s.userId).Sum(i => i.Quantity)
            }).ToList();

            return Ok(new
            {
                TopSellingProducts = topSelling,
                MostFavouritedProducts = topFavourites,
                SellerPerformance = sellerStats
            });
        }
    }
}
