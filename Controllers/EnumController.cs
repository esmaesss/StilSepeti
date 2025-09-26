using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.Enums;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnumController : ControllerBase
    {
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = Enum.GetNames(typeof(Role)).ToList();
            return Ok(roles);
        }

        [HttpGet("order-status")]
        public IActionResult GetOrderStatuses()
        {
            var statuses = Enum.GetNames(typeof(OrderStatus)).ToList();
            return Ok(statuses);
        }

        [HttpGet("return-status")]
        public IActionResult GetReturnStatuses()
        {
            var statuses = Enum.GetNames(typeof(ReturnStatus)).ToList();
            return Ok(statuses);
        }
    }

}
