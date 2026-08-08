using SewingAPI.Service.DTOs.Fill;

namespace SewingAPI.Service.Mapping
{
    public static class FillMapper
    {
        public static FillResponseDto ToResponse(Domain.Entities.Fill fill)
        {
            return new FillResponseDto
            {
                Id = fill.Id,
                Type = fill.Type,
                Description = fill.Description,
                Related = fill.Related,
                Status = fill.Status,
                CreatedAt = fill.CreatedAt
            };
        }
    }
}
