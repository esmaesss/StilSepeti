using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.Models;
using System.Security.Claims;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/member/addresses")]
    [Authorize(Roles = "Member")]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] Address address)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            address.UserId = userId;

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return Ok("Adres başarıyla eklendi.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return Ok(addresses);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == id);

            if (address == null)
                return NotFound("Adres bulunamadı.");

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok("Adres silindi.");
        }
    }
}
