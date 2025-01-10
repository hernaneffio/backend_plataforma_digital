using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;
using Domain.Payload.Authentication;

namespace Metroli_PDF.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> loginOwner([FromBody] LoginPayload payload) => Ok(await _authenticationService.Login(payload));

        [HttpPost("reflesh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefleshTokenPayload payload) => Ok(await _authenticationService.refleshLogin(payload));
    }
}
