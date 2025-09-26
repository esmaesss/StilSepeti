using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.DTO;


namespace StilSepetiApp.Controllers
{
    [Authorize(Roles ="Admin")]
    [ApiController]
    [Route("api/admin/orders")]

    public class AdminOrderController: ControllerBase
    {
        private readonly IOrderService _orderService;
        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;

        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusdto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.NewStatus);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}

