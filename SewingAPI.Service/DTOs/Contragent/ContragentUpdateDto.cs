using SewingAPI.Domain.Enums;

namespace SewingAPI.Service.DTOs.Contragent
{
    public class ContragentUpdateDto
    {
        public ContragentType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
    }
}
