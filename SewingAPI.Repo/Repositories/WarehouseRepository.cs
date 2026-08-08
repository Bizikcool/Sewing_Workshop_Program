using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Repo.Data;

namespace SewingAPI.Repo.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _db;

        public WarehouseRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<WarehouseItem> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? contragentType, MaterialType? materialType)
        {
            var query = _db.WarehouseItems
                .AsNoTracking()
                .Include(e => e.Material)
                .Include(e => e.Contragent)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(e =>
                    (e.Material != null && e.Material.Name.ToLower().Contains(term))
                    || (e.Material != null && e.Material.Article != null && e.Material.Article.ToLower().Contains(term))
                    || (e.Material != null && e.Material.Color != null && e.Material.Color.ToLower().Contains(term))
                    || (e.Contragent != null && e.Contragent.Name.ToLower().Contains(term)));
            }

            if (contragentType.HasValue)
            {
                query = query.Where(e => e.Contragent != null && e.Contragent.Type == contragentType.Value);
            }

            if (materialType.HasValue)
            {
                query = query.Where(e => e.Material != null && e.Material.Type == materialType.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.Material != null ? e.Material.Name : string.Empty)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<WarehouseItem?> GetByIdAsync(int id)
        {
            return _db.WarehouseItems
                .Include(e => e.Material)
                .Include(e => e.Contragent)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task<WarehouseItem?> GetByMaterialIdAsync(int materialId)
        {
            return _db.WarehouseItems
                .Include(e => e.Material)
                .Include(e => e.Contragent)
                .FirstOrDefaultAsync(e => e.MaterialId == materialId);
        }

        public async Task<WarehouseItem> AddAsync(WarehouseItem item)
        {
            _db.WarehouseItems.Add(item);
            await _db.SaveChangesAsync();
            await LoadReferences(item);
            return item;
        }

        public async Task<WarehouseItem> UpdateAsync(WarehouseItem item)
        {
            _db.WarehouseItems.Update(item);
            await _db.SaveChangesAsync();
            await LoadReferences(item);
            return item;
        }

        public async Task<WarehouseItem> UpdateQtyAsync(int id, decimal newQty)
        {
            var item = await GetByIdAsync(id);
            if (item is null)
            {
                throw new InvalidOperationException($"Warehouse item {id} not found.");
            }

            item.Qty = newQty;
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _db.WarehouseItems.FindAsync(id);
            if (item is null)
            {
                return false;
            }

            _db.WarehouseItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task LoadReferences(WarehouseItem item)
        {
            await _db.Entry(item).Reference(e => e.Material).LoadAsync();
            await _db.Entry(item).Reference(e => e.Contragent).LoadAsync();
        }
    }
}
