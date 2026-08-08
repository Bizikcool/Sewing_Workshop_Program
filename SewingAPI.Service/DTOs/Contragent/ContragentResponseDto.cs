using SewingAPI.Domain.Enums;

namespace SewingAPI.Service.DTOs.Contragent
{
    public class ContragentResponseDto
    {
        public int Id { get; set; }
        public ContragentType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MaterialsCount { get; set; }
        public int OrdersCount { get; set; }
    }
}
