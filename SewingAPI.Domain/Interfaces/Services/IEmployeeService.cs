using SewingAPI.Domain.Entities;

namespace SewingAPI.Domain.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<(IReadOnlyList<Employee> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, string? position, int? rank);
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(int id, Employee employee);
        Task<bool> DeleteAsync(int id);
    }
}
