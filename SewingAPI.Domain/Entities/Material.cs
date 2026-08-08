using SewingAPI.Domain.Enums;

namespace SewingAPI.Domain.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public int ContragentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public MaterialType Type { get; set; }
        public string? Color { get; set; }
        public string? Article { get; set; }
        public string Unit { get; set; } = "шт";
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }

        public Contragent? Contragent { get; set; }
        public WarehouseItem? WarehouseItem { get; set; }
    }
}
