using SewingAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Interfaces.Repositories
{
    public interface IMaterialRepository
    {
        Task<(IReadOnlyList<Material> Items, int Total)> GetPagedByContragentAsync(int contragentId, int page, int pageSize);
        Task<Material?> GetByIdAsync(int id);
        Task<Material> AddAsync(Material material);
        Task<Material> UpdateAsync(Material material);
        Task<bool> DeleteAsync(int id);
    }
}
