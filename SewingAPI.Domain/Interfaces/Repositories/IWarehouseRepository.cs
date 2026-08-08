using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;

namespace SewingAPI.Domain.Interfaces.Repositories
{
    public interface IWarehouseRepository
    {
        Task<(IReadOnlyList<WarehouseItem> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? contragentType, MaterialType? materialType);
        Task<WarehouseItem?> GetByIdAsync(int id);
        Task<WarehouseItem?> GetByMaterialIdAsync(int materialId);
        Task<WarehouseItem> AddAsync(WarehouseItem item);
        Task<WarehouseItem> UpdateAsync(WarehouseItem item);
        Task<WarehouseItem> UpdateQtyAsync(int id, decimal newQty);
        Task<bool> DeleteAsync(int id);
    }
}
