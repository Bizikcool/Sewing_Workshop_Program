using SewingAPI.Domain.Entities;

namespace SewingAPI.Domain.Interfaces.Services
{
    public interface IMaterialService
    {
        Task<(IReadOnlyList<Material> Items, int Total)> GetPagedByContragentAsync(int contragentId, int page, int pageSize);
        Task<Material> GetByIdAsync(int id);
        Task<Material> CreateAsync(Material material);
        Task<Material> UpdateAsync(int id, Material material);
        Task<bool> DeleteAsync(int id);
    }
}
