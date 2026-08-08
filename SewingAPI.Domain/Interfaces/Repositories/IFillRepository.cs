using SewingAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Interfaces.Repositories
{
    public interface IFillRepository
    {
        Task<(IReadOnlyList<Fill> Items, int Total)> GetPagedAsync(int page, int pageSize, string? type, DateTime? dateFrom, DateTime? dateTo);
        Task<Fill> AddAsync(Fill fill);
        Task<(int TotalCount, int TodayCount)> GetStatsAsync();
    }
}
