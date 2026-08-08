using SewingAPI.Service.DTOs.Material;

namespace SewingAPI.Service.Mapping
{
    public static class MaterialMapper
    {
        public static Domain.Entities.Material ToEntity(MaterialCreateDto dto)
        {
            return new Domain.Entities.Material
            {
                ContragentId = dto.ContragentId,
                Name = dto.Name,
                Type = dto.Type,
                Color = dto.Color,
                Article = dto.Article,
                Unit = dto.Unit,
                Qty = dto.Qty,
                Price = dto.Price
            };
        }

        public static Domain.Entities.Material ToEntity(MaterialUpdateDto dto)
        {
            return new Domain.Entities.Material
            {
                ContragentId = dto.ContragentId,
                Name = dto.Name,
                Type = dto.Type,
                Color = dto.Color,
                Article = dto.Article,
                Unit = dto.Unit,
                Qty = dto.Qty,
                Price = dto.Price
            };
        }

        public static MaterialResponseDto ToResponse(Domain.Entities.Material material)
        {
            return new MaterialResponseDto
            {
                Id = material.Id,
                ContragentId = material.ContragentId,
                ContragentName = material.Contragent?.Name,
                Name = material.Name,
                Type = material.Type,
                Color = material.Color,
                Article = material.Article,
                Unit = material.Unit,
                Qty = material.Qty,
                Price = material.Price,
                Sum = material.Sum
            };
        }
    }
}
