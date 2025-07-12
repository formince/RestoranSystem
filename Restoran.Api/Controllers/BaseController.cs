using Microsoft.AspNetCore.Mvc;

namespace Restoran.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected string WebRootPath
        {
            get
            {
                var env = HttpContext.RequestServices.GetService<IWebHostEnvironment>();
                return env?.WebRootPath ?? "";
            }
        }
        
        protected IActionResult HandleResult<T>(T result, string message = "")
        {
            if (result == null)
                return NotFound(new { success = false, message = "Veri bulunamadı" });

            return Ok(new { success = true, data = result, message });
        }

        protected IActionResult HandleResult(bool success, string message)
        {
            if (success)
                return Ok(new { success = true, message });
            
            return BadRequest(new { success = false, message });
        }

        protected IActionResult HandleResult((bool Success, string Message) result)
        {
            return HandleResult(result.Success, result.Message);
        }
    }
} 