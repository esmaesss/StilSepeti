using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.Models;
using System.Security.Claims;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/member/cards")]
    [Authorize(Roles = "Member")]
    public class CardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] Card card)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            card.UserId = userId;

            var exists = await _context.Cards
                .AnyAsync(c => c.UserId == userId && c.CardNumber == card.CardNumber);

            if (exists)
                return BadRequest("Bu kart zaten kayıtlı.");

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return Ok("Kart başarıyla eklendi.");
        }

        [HttpGet]
        public async Task<IActionResult> GetCards()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cards = await _context.Cards
                .Where(c => c.UserId == userId)
                .Select(c => new { c.Id, c.CardNumber }) // Şifre gösterilmez
                .ToListAsync();

            return Ok(cards);
        }
    }
}
