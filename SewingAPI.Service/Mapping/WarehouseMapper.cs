using SewingAPI.Service.DTOs.Warehouse;

namespace SewingAPI.Service.Mapping
{
    public static class WarehouseMapper
    {
        public static WarehouseResponseDto ToResponse(Domain.Entities.WarehouseItem item)
        {
            return new WarehouseResponseDto
            {
                Id = item.Id,
                MaterialId = item.MaterialId,
                MaterialName = item.Material?.Name,
                MaterialType = item.Material?.Type,
                ContragentId = item.ContragentId,
                ContragentName = item.Contragent?.Name,
                Qty = item.Qty,
                Price = item.Price,
                DateAdded = item.DateAdded
            };
        }
    }
}
