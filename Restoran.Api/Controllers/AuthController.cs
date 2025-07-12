using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.User;
using System.Security.Claims;

namespace Restoran.Api.Controllers
{
    public class AuthController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authBLL = new BLLAuth();
            var result = await authBLL.RegisterAsync(dto);
            return HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authBLL = new BLLAuth();
            var result = await authBLL.LoginWithJwtAsync(dto);
            
            if (!result.Success)
                return HandleResult(result.Success, result.Message);

            // Kullanıcı bilgilerini al
            var user = await authBLL.LoginAsync(dto);
            
            return Ok(new 
            { 
                success = true, 
                message = result.Message,
                data = new 
                {
                    user = user,
                    token = result.Token
                }
            });
        }


    }
} 