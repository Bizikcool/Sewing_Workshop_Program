using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Exceptions;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Domain.Interfaces.Services;

namespace SewingAPI.Service.Services
{
    public class FillService : IFillService
    {
        private readonly IFillRepository _fills;

        public FillService(IFillRepository fills)
        {
            _fills = fills;
        }

        public Task<(IReadOnlyList<Fill> Items, int Total)> GetPagedAsync(int page, int pageSize, string? type, DateTime? dateFrom, DateTime? dateTo)
        {
            return _fills.GetPagedAsync(NormalizePage(page), NormalizePageSize(pageSize), type, dateFrom, dateTo);
        }

        public Task<Fill> CreateAsync(string type, string description, string related)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ValidationException("Тип события обязателен.");
            }

            return _fills.AddAsync(new Fill
            {
                Type = type,
                Description = description,
                Related = related,
                Status = "Выполнено",
                CreatedAt = DateTime.UtcNow
            });
        }

        public Task<(int TotalCount, int TodayCount)> GetStatsAsync()
        {
            return _fills.GetStatsAsync();
        }

        private static int NormalizePage(int page) => page < 1 ? 1 : page;

        private static int NormalizePageSize(int pageSize) => pageSize switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => pageSize
        };
    }
}
