using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Interfaces.Repositories
{
    public interface IContragentRepository
    {
        Task<(IReadOnlyList<Contragent> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? type);
        Task<Contragent?> GetByIdWithDetailsAsync(int id);
        Task<Contragent> AddAsync(Contragent contragent);
        Task<Contragent> UpdateAsync(Contragent contragent);
        Task<bool> DeleteAsync(int id);
    }
}
