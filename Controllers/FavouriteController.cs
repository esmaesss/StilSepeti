using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Models;

namespace StilSepetiApp.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Member")]
    public class FavouriteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FavouriteController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToFavourites(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var exists = await _context.Favourites
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

            if (exists)
                return BadRequest("Bu ürün zaten favorilerde.");

            var favourite = new Favourite
            {
                UserId = userId,
                ProductId = productId
            };

            _context.Favourites.Add(favourite);
            await _context.SaveChangesAsync();

            return Ok("Ürün favorilere eklendi.");
        }

        [HttpGet]
        public async Task<IActionResult> GetFavourites()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var favourites = await _context.Favourites
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            var dtoList = favourites.Select(f => new Favouritedto
            {
                ProductId = f.ProductId,
                ProductName = f.Product.Name,
                ImageUrl = f.Product.ImageUrl,
                Price = f.Product.Price
            }).ToList();

            return Ok(dtoList);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromFavourites(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var favourite = await _context.Favourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favourite == null)
                return NotFound("Favori bulunamadı.");

            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();

            return Ok("Favori kaldırıldı.");
        }
    }

}
