using SewingAPI.Domain.Entities;

namespace SewingAPI.Domain.Interfaces.Services
{
    public interface IFillService
    {
        Task<(IReadOnlyList<Fill> Items, int Total)> GetPagedAsync(int page, int pageSize, string? type, DateTime? dateFrom, DateTime? dateTo);
        Task<Fill> CreateAsync(string type, string description, string related);
        Task<(int TotalCount, int TodayCount)> GetStatsAsync();
    }
}
