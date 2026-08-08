using SewingAPI.Domain.Enums;

namespace SewingAPI.Service.DTOs.Warehouse
{
    public class WarehouseResponseDto
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public MaterialType? MaterialType { get; set; }
        public int ContragentId { get; set; }
        public string? ContragentName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public DateOnly DateAdded { get; set; }
    }
}
