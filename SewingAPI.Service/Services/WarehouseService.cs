using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;
using SewingAPI.Domain.Exceptions;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Domain.Interfaces.Services;

namespace SewingAPI.Service.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouse;

        public WarehouseService(IWarehouseRepository warehouse)
        {
            _warehouse = warehouse;
        }

        public Task<(IReadOnlyList<WarehouseItem> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? contragentType, MaterialType? materialType)
        {
            return _warehouse.GetPagedAsync(NormalizePage(page), NormalizePageSize(pageSize), search, contragentType, materialType);
        }

        public async Task<WarehouseItem> GetByIdAsync(int id)
        {
            return await _warehouse.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(WarehouseItem), id);
        }

        public async Task<WarehouseItem> UpdateQuantityAsync(int id, decimal newQty)
        {
            if (newQty < 0)
            {
                throw new BusinessException("Количество на складе не может быть отрицательным.");
            }

            await GetByIdAsync(id);
            return await _warehouse.UpdateQtyAsync(id, newQty);
        }

        public async Task<WarehouseItem> DeductQuantityAsync(int id, decimal amount)
        {
            if (amount <= 0)
            {
                throw new BusinessException("Количество для списания должно быть больше 0.");
            }

            var item = await GetByIdAsync(id);
            if (item.Qty < amount)
            {
                throw new BusinessException("Недостаточно материала на складе.");
            }

            return await _warehouse.UpdateQtyAsync(id, item.Qty - amount);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _warehouse.DeleteAsync(id);
            if (!deleted)
            {
                throw new NotFoundException(nameof(WarehouseItem), id);
            }

            return true;
        }

        private static int NormalizePage(int page) => page < 1 ? 1 : page;

        private static int NormalizePageSize(int pageSize) => pageSize switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => pageSize
        };
    }
}
