using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.DTO;
using StilSepetiApp.Services;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/member/products")]
    [Authorize(Roles = "Member")]
    public class MemberProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public MemberProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilter filter)
        {
            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }
    }

}
