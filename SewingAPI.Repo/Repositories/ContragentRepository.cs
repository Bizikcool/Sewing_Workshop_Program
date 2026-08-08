using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Repo.Data;

namespace SewingAPI.Repo.Repositories
{
    public class ContragentRepository : IContragentRepository
    {
        private readonly AppDbContext _db;

        public ContragentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<Contragent> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? type)
        {
            var query = _db.Contragents
                .AsNoTracking()
                .Include(e => e.Materials)
                .Include(e => e.Orders)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(term)
                    || (e.Contact != null && e.Contact.ToLower().Contains(term))
                    || (e.Phone != null && e.Phone.ToLower().Contains(term)));
            }

            if (type.HasValue)
            {
                query = query.Where(e => e.Type == type.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<Contragent?> GetByIdWithDetailsAsync(int id)
        {
            return _db.Contragents
                .Include(e => e.Materials)
                .Include(e => e.Orders)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Contragent> AddAsync(Contragent contragent)
        {
            _db.Contragents.Add(contragent);
            await _db.SaveChangesAsync();
            return contragent;
        }

        public async Task<Contragent> UpdateAsync(Contragent contragent)
        {
            _db.Contragents.Update(contragent);
            await _db.SaveChangesAsync();
            return contragent;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contragent = await _db.Contragents.FindAsync(id);
            if (contragent is null)
            {
                return false;
            }

            _db.Contragents.Remove(contragent);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
