namespace SewingAPI.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> RegisterAsync(string username, string password);
        Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> LoginAsync(string username, string password);
        Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Username)> RefreshAsync(string refreshToken);
    }
}
