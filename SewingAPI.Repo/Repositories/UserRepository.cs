using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Repo.Data;

namespace SewingAPI.Repo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<AppUser?> GetByUsernameAsync(string username)
        {
            var normalizedUsername = username.Trim().ToLower();
            return _db.Users.FirstOrDefaultAsync(e => e.Username.ToLower() == normalizedUsername);
        }

        public Task<AppUser?> GetByRefreshTokenAsync(string refreshToken)
        {
            return _db.Users.FirstOrDefaultAsync(e => e.RefreshToken == refreshToken);
        }

        public async Task<AppUser> AddAsync(AppUser user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<AppUser> UpdateAsync(AppUser user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public Task<bool> ExistsAsync(string username)
        {
            var normalizedUsername = username.Trim().ToLower();
            return _db.Users.AnyAsync(e => e.Username.ToLower() == normalizedUsername);
        }
    }
}
