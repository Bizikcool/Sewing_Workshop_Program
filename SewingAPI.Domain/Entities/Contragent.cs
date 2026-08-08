using SewingAPI.Domain.Enums;

namespace SewingAPI.Domain.Entities
{
    public class Contragent
    {
        public int Id { get; set; }
        public ContragentType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Material> Materials { get; set; } = new List<Material>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
