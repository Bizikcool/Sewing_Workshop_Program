using SewingAPI.Domain.Enums;

namespace SewingAPI.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int ContragentId { get; set; }
        public string? Num { get; set; }
        public DateOnly? Date { get; set; }
        public DateOnly? Deadline { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.InProgress;

        public Contragent? Contragent { get; set; }
    }
}
