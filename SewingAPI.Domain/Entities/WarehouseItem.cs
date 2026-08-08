namespace SewingAPI.Domain.Entities
{
    public class WarehouseItem
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int ContragentId { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public DateOnly DateAdded { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public Material? Material { get; set; }
        public Contragent? Contragent { get; set; }
    }
}
