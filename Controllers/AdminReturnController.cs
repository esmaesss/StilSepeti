using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.Services;
using StilSepetiApp.DTO;

namespace StilSepetiApp.Controllers
{


    [Authorize(Roles = "Admin")]
    [ApiController]
   [Route("api/admin-return-requests")]

    public class AdminReturnController : ControllerBase
    {
        private readonly IReturnService _returnService;

        public AdminReturnController(IReturnService returnService)
        {
            _returnService = returnService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var returns = await _returnService.GetAllAsync();
            return Ok(returns);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReturnStatusdto dto)
        {
            var result = await _returnService.UpdateStatusAsync(id, dto.NewStatus);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}