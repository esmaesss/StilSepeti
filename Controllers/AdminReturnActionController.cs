using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
using StilSepetiApp.Models;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/admin/returns")]
    [Authorize(Roles = "Admin")]
    public class AdminReturnActionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminReturnActionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("approve/{ReturnId}")]
        public async Task<IActionResult> ApproveReturn(int ReturnId)
        {
            var returnRequest = await _context.ReturnRequests.FindAsync(ReturnId);
            if (returnRequest == null)
                return NotFound("İade talebi bulunamadı.");

            returnRequest.Status = ReturnStatus.Approved;
            returnRequest.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("İade talebi onaylandı.");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectReturn([FromBody] RejectReturnRequestdto dto)
        {
            var returnRequest = await _context.ReturnRequests.FindAsync(dto.ReturnId);
            if (returnRequest == null)
                return NotFound("İade talebi bulunamadı.");

            returnRequest.Status = ReturnStatus.Rejected;
            returnRequest.RejectionReason = dto.Reason;
            returnRequest.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("İade talebi reddedildi.");

        }
    }
}