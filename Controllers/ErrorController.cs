using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("error")]
    [ApiExplorerSettings(IgnoreApi = true)] 
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            _logger.LogError(exception, "Bir hata oluştu");

           
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            var response = new
            {
                Message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
                Detail = isDevelopment ? exception?.Message : null,
                StackTrace = isDevelopment ? exception?.StackTrace : null
            };

            return Problem(
                title: response.Message,
                detail: isDevelopment ? response.Detail : null,
                statusCode: 500
            );
        }
    }
}
