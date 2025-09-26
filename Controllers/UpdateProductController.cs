using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.DTO;
using StilSepetiApp.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace StilSepetiApp.Controllers
{
    [Authorize(Roles = "Seller")]
    [ApiController]
    [Route("api/update-product")]
    public class UpdateProductController : Controller
    {
        private readonly IProductService _productService;

        public UpdateProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdatedto dto)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _productService.UpdateProductAsync(id, dto, sellerId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
