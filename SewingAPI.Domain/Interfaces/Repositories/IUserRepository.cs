using SewingAPI.Domain.Entities;

namespace SewingAPI.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<AppUser?> GetByRefreshTokenAsync(string refreshToken);
        Task<AppUser> AddAsync(AppUser user);
        Task<AppUser> UpdateAsync(AppUser user);
        Task<bool> ExistsAsync(string username);
    }
}
