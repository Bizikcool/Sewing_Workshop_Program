using SewingAPI.Service.DTOs.Contragent;

namespace SewingAPI.Service.Mapping
{
    public static class ContragentMapper
    {
        public static Domain.Entities.Contragent ToEntity(ContragentCreateDto dto)
        {
            return new Domain.Entities.Contragent
            {
                Type = dto.Type,
                Name = dto.Name,
                Contact = dto.Contact,
                Phone = dto.Phone
            };
        }

        public static Domain.Entities.Contragent ToEntity(ContragentUpdateDto dto)
        {
            return new Domain.Entities.Contragent
            {
                Type = dto.Type,
                Name = dto.Name,
                Contact = dto.Contact,
                Phone = dto.Phone
            };
        }

        public static ContragentResponseDto ToResponse(Domain.Entities.Contragent contragent)
        {
            return new ContragentResponseDto
            {
                Id = contragent.Id,
                Type = contragent.Type,
                Name = contragent.Name,
                Contact = contragent.Contact,
                Phone = contragent.Phone,
                CreatedAt = contragent.CreatedAt,
                MaterialsCount = contragent.Materials.Count,
                OrdersCount = contragent.Orders.Count
            };
        }
    }
}
