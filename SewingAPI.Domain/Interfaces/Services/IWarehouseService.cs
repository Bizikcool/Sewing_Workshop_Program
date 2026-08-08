using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;

namespace SewingAPI.Domain.Interfaces.Services
{
    public interface IWarehouseService
    {
        Task<(IReadOnlyList<WarehouseItem> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? contragentType, MaterialType? materialType);
        Task<WarehouseItem> GetByIdAsync(int id);
        Task<WarehouseItem> UpdateQuantityAsync(int id, decimal newQty);
        Task<WarehouseItem> DeductQuantityAsync(int id, decimal amount);
        Task<bool> DeleteAsync(int id);
    }
}
