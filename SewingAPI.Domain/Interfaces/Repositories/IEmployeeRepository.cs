using SewingAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<(IReadOnlyList<Employee> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, string? position, int? rank);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
    }
}
