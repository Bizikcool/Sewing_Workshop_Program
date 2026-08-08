using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Exceptions;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.Validation;

namespace SewingAPI.Service.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materials;
        private readonly IContragentRepository _contragents;
        private readonly IWarehouseRepository _warehouse;
        private readonly IFillRepository _fills;

        public MaterialService(
            IMaterialRepository materials,
            IContragentRepository contragents,
            IWarehouseRepository warehouse,
            IFillRepository fills)
        {
            _materials = materials;
            _contragents = contragents;
            _warehouse = warehouse;
            _fills = fills;
        }

        public Task<(IReadOnlyList<Material> Items, int Total)> GetPagedByContragentAsync(int contragentId, int page, int pageSize)
        {
            if (contragentId <= 0)
            {
                throw new ValidationException("Контрагент обязателен.");
            }

            return _materials.GetPagedByContragentAsync(contragentId, NormalizePage(page), NormalizePageSize(pageSize));
        }

        public async Task<Material> GetByIdAsync(int id)
        {
            return await _materials.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Material), id);
        }

        public async Task<Material> CreateAsync(Material material)
        {
            MaterialValidator.Validate(material);
            await EnsureContragentExists(material.ContragentId);

            material.Sum = material.Qty * material.Price;
            var created = await _materials.AddAsync(material);

            created.WarehouseItem = await _warehouse.AddAsync(new WarehouseItem
            {
                MaterialId = created.Id,
                ContragentId = created.ContragentId,
                Qty = created.Qty,
                Price = created.Price,
                DateAdded = DateOnly.FromDateTime(DateTime.Today)
            });

            await _fills.AddAsync(new Fill
            {
                Type = "Материал",
                Description = $"Создан материал {created.Name}",
                Related = $"Material:{created.Id}"
            });

            return created;
        }

        public async Task<Material> UpdateAsync(int id, Material material)
        {
            MaterialValidator.Validate(material);
            await EnsureContragentExists(material.ContragentId);

            var existing = await GetByIdAsync(id);
            existing.ContragentId = material.ContragentId;
            existing.Name = material.Name;
            existing.Type = material.Type;
            existing.Color = material.Color;
            existing.Article = material.Article;
            existing.Unit = material.Unit;
            existing.Qty = material.Qty;
            existing.Price = material.Price;
            existing.Sum = material.Qty * material.Price;

            var updated = await _materials.UpdateAsync(existing);
            var warehouseItem = await _warehouse.GetByMaterialIdAsync(updated.Id);

            if (warehouseItem is null)
            {
                updated.WarehouseItem = await _warehouse.AddAsync(new WarehouseItem
                {
                    MaterialId = updated.Id,
                    ContragentId = updated.ContragentId,
                    Qty = updated.Qty,
                    Price = updated.Price,
                    DateAdded = DateOnly.FromDateTime(DateTime.Today)
                });
            }
            else
            {
                warehouseItem.ContragentId = updated.ContragentId;
                warehouseItem.Qty = updated.Qty;
                warehouseItem.Price = updated.Price;
                updated.WarehouseItem = await _warehouse.UpdateAsync(warehouseItem);
            }

            await _fills.AddAsync(new Fill
            {
                Type = "Материал",
                Description = $"Обновлен материал {updated.Name}",
                Related = $"Material:{updated.Id}"
            });

            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await GetByIdAsync(id);
            var deleted = await _materials.DeleteAsync(id);
            if (!deleted)
            {
                throw new NotFoundException(nameof(Material), id);
            }

            await _fills.AddAsync(new Fill
            {
                Type = "Материал",
                Description = $"Удален материал {existing.Name}",
                Related = $"Material:{id}"
            });

            return true;
        }

        private async Task EnsureContragentExists(int contragentId)
        {
            if (await _contragents.GetByIdWithDetailsAsync(contragentId) is null)
            {
                throw new NotFoundException(nameof(Contragent), contragentId);
            }
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
