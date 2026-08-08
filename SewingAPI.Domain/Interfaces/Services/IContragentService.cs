using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;

namespace SewingAPI.Domain.Interfaces.Services
{
    public interface IContragentService
    {
        Task<(IReadOnlyList<Contragent> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? type);
        Task<Contragent> GetByIdAsync(int id);
        Task<Contragent> CreateAsync(Contragent contragent);
        Task<Contragent> UpdateAsync(int id, Contragent contragent);
        Task<bool> DeleteAsync(int id);
    }
}
