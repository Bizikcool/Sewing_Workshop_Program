using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.DTOs.Auth;

namespace SewingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request.Username, request.Password);
            return Ok(ToResponse(result));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Username, request.Password);
            return Ok(ToResponse(result));
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(ToResponse(result));
        }

        private static AuthResponse ToResponse((string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username) result)
        {
            return new AuthResponse
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ExpiresAt = result.ExpiresAt,
                Username = result.Username
            };
        }
    }
}
