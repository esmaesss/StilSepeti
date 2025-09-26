
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.Enums;
using StilSepetiApp.Models;




namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReturnController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReturnController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{orderId}")]
        public async Task<IActionResult> RequestReturn(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return NotFound("Sipariş bulunamadı.");

            var timeSinceOrder = DateTime.UtcNow - order.CreatedAt;
            if (timeSinceOrder.TotalDays > 14)
                return BadRequest("İade süresi (14 gün) dolmuş.");

            if (order.Status != OrderStatus.Delivered)
                return BadRequest("Yalnızca teslim edilmiş siparişler iade edilebilir.");

            var returnRequest = new ReturnRequest
            {
                OrderId = order.Id,
                UserId = userId,
                Reason = "Müşteri tarafından talep edildi",
                Status = ReturnStatus.Requested
            };

            _context.ReturnRequests.Add(returnRequest);
            await _context.SaveChangesAsync();

            return Ok("İade talebi başarıyla oluşturuldu.");
        }
    }


}

