using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StilSepetiApp.DTO;
using StilSepetiApp.Services;
using System.Security.Claims;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] CreatePaymentRequestdto paymentRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _paymentService.ProcessPaymentAsync(paymentRequest, userId);

            if (!result.Success)
                return BadRequest(result.Message);

            _logger.LogInformation("Ödeme işlemi başarılı: PaymentId={PaymentId}, UserId={UserId}", result.Data.Id, userId);

            return Ok(result.Data);
        }

        [HttpGet("{paymentId}/status")]
        public async Task<IActionResult> GetPaymentStatus(int paymentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentId);

            if (paymentStatus == null)
                return NotFound("Ödeme kaydı bulunamadı.");

          

            return Ok(paymentStatus);
        }

        [HttpPost("{paymentId}/refund")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefundPayment(int paymentId, [FromBody] decimal amount)
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId, amount);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}