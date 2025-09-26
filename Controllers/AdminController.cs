using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.Enums;
using StilSepetiApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;



namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/admin")] 
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;

        }
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalReturns = await _context.ReturnRequests.CountAsync();
            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 10)
                .Select(p => new { p.Name, p.Stock })
                .ToListAsync();

            var topSelling = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                totalOrders,
                totalReturns,
                lowStockProducts,
                topSelling
            });
        }
        [HttpGet("dashboard/enhanced-stats")]
        public async Task<IActionResult> GetEnhancedDashboardStats()
        {
            var totalSales = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.StoredTotalAmount);

            var today = DateTime.UtcNow.Date;
            var todaysOrders = await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == today);

            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 5)
                .CountAsync();

            var newUsersThisMonth = await _context.Users
                .CountAsync(u => u.CreatedAt.Month == today.Month && u.CreatedAt.Year == today.Year);

            return Ok(new
            {
                TotalRevenue = totalSales,
                TodaysOrderCount = todaysOrders,
                LowStockAlertCount = lowStockProducts,
                NewUsersThisMonth = newUsersThisMonth
            });
        }


        [HttpGet("returns")]
        public async Task<IActionResult> GetAllReturnRequests()
        {
            var returns = await _context.ReturnRequests
                .Include(r => r.Order)
                .Include(r => r.User)
                .ToListAsync();

            return Ok(returns);
        }
    }
}