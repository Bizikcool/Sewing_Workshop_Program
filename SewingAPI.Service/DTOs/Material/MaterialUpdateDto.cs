using SewingAPI.Domain.Enums;

namespace SewingAPI.Service.DTOs.Material
{
    public class MaterialUpdateDto
    {
        public int ContragentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public MaterialType Type { get; set; }
        public string? Color { get; set; }
        public string? Article { get; set; }
        public string Unit { get; set; } = "шт";
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
    }
}
