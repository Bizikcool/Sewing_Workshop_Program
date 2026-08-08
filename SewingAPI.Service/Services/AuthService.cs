using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Exceptions;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.Validation;

namespace SewingAPI.Service.Services
{
    public class AuthService : IAuthService
    {
        private const int RefreshTokenDays = 7;
        private const int PasswordIterations = 100_000;

        private readonly IUserRepository _users;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository users, IConfiguration configuration)
        {
            _users = users;
            _configuration = configuration;
        }

        public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> RegisterAsync(string username, string password)
        {
            AuthValidator.ValidateCredentials(username, password);

            var normalizedUsername = username.Trim();
            if (await _users.ExistsAsync(normalizedUsername))
            {
                throw new BusinessException("Пользователь с таким именем уже существует.");
            }

            var user = await _users.AddAsync(new AppUser
            {
                Username = normalizedUsername,
                PasswordHash = HashPassword(password),
                Role = "user",
                CreatedAt = DateTime.UtcNow
            });

            return await IssueTokensAsync(user);
        }

        public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> LoginAsync(string username, string password)
        {
            AuthValidator.ValidateCredentials(username, password);

            var user = await _users.GetByUsernameAsync(username.Trim());
            if (user is null || !VerifyPassword(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль.");
            }

            return await IssueTokensAsync(user);
        }

        public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token обязателен.");
            }

            var user = await _users.GetByRefreshTokenAsync(refreshToken);
            if (user is null || user.TokenExpiresAt is null || user.TokenExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token недействителен или истек.");
            }

            return await IssueTokensAsync(user);
        }

        private async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> IssueTokensAsync(AppUser user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenMinutes());
            var accessToken = GenerateAccessToken(user, expiresAt);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.TokenExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays);
            await _users.UpdateAsync(user);

            return (accessToken, refreshToken, expiresAt, user.Username);
        }

        private string GenerateAccessToken(AppUser user, DateTime expiresAt)
        {
            var header = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new
            {
                alg = "HS256",
                typ = "JWT"
            }));

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var payload = new Dictionary<string, object?>
            {
                ["sub"] = user.Id.ToString(),
                ["unique_name"] = user.Username,
                ["nameid"] = user.Id.ToString(),
                ["name"] = user.Username,
                ["role"] = user.Role,
                ["iat"] = now,
                ["nbf"] = now,
                ["exp"] = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
            };

            var issuer = _configuration["Jwt:Issuer"];
            if (!string.IsNullOrWhiteSpace(issuer))
            {
                payload["iss"] = issuer;
            }

            var audience = _configuration["Jwt:Audience"];
            if (!string.IsNullOrWhiteSpace(audience))
            {
                payload["aud"] = audience;
            }

            var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
            var unsignedToken = $"{header}.{encodedPayload}";
            var signature = Sign(unsignedToken);

            return $"{unsignedToken}.{signature}";
        }

        private int GetAccessTokenMinutes()
        {
            return int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var minutes) && minutes > 0
                ? minutes
                : 60;
        }

        private string GetJwtKey()
        {
            return _configuration["Jwt:Key"] ?? "dev-secret-key-change-me-please-32chars";
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string Sign(string value)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GetJwtKey()));
            return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, PasswordIterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return $"PBKDF2${PasswordIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('$');
            if (parts.Length != 4 || parts[0] != "PBKDF2" || !int.TryParse(parts[1], out var iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[2]);
            var expectedHash = Convert.FromBase64String(parts[3]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var actualHash = pbkdf2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
