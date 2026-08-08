using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Repo.Data;

namespace SewingAPI.Repo.Repositories
{
    public class FillRepository : IFillRepository
    {
        private readonly AppDbContext _db;

        public FillRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<Fill> Items, int Total)> GetPagedAsync(int page, int pageSize, string? type, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _db.Fills.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                var normalizedType = type.Trim().ToLower();
                query = query.Where(e => e.Type.ToLower().Contains(normalizedType));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.CreatedAt >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.CreatedAt <= dateTo.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Fill> AddAsync(Fill fill)
        {
            _db.Fills.Add(fill);
            await _db.SaveChangesAsync();
            return fill;
        }

        public async Task<(int TotalCount, int TodayCount)> GetStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var total = await _db.Fills.CountAsync();
            var todayCount = await _db.Fills.CountAsync(e => e.CreatedAt >= today && e.CreatedAt < tomorrow);

            return (total, todayCount);
        }
    }
}
