using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Repo.Data;

namespace SewingAPI.Repo.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _db;

        public MaterialRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<Material> Items, int Total)> GetPagedByContragentAsync(int contragentId, int page, int pageSize)
        {
            var query = _db.Materials
                .AsNoTracking()
                .Include(e => e.Contragent)
                .Include(e => e.WarehouseItem)
                .Where(e => e.ContragentId == contragentId);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<Material?> GetByIdAsync(int id)
        {
            return _db.Materials
                .Include(e => e.Contragent)
                .Include(e => e.WarehouseItem)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Material> AddAsync(Material material)
        {
            _db.Materials.Add(material);
            await _db.SaveChangesAsync();
            await _db.Entry(material).Reference(e => e.Contragent).LoadAsync();
            return material;
        }

        public async Task<Material> UpdateAsync(Material material)
        {
            _db.Materials.Update(material);
            await _db.SaveChangesAsync();
            await _db.Entry(material).Reference(e => e.Contragent).LoadAsync();
            return material;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var material = await _db.Materials.FindAsync(id);
            if (material is null)
            {
                return false;
            }

            _db.Materials.Remove(material);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
