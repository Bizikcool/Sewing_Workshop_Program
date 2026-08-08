using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Exceptions;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.Validation;

namespace SewingAPI.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employees;

        public EmployeeService(IEmployeeRepository employees)
        {
            _employees = employees;
        }

        public Task<(IReadOnlyList<Employee> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, string? position, int? rank)
        {
            return _employees.GetPagedAsync(NormalizePage(page), NormalizePageSize(pageSize), search, position, rank);
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _employees.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Employee), id);
        }

        public Task<Employee> CreateAsync(Employee employee)
        {
            EmployeeValidator.Validate(employee);
            employee.CreatedAt = DateTime.UtcNow;
            return _employees.AddAsync(employee);
        }

        public async Task<Employee> UpdateAsync(int id, Employee employee)
        {
            EmployeeValidator.Validate(employee);

            var existing = await GetByIdAsync(id);
            existing.Fio = employee.Fio;
            existing.Position = employee.Position;
            existing.Rank = employee.Rank;

            return await _employees.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _employees.DeleteAsync(id);
            if (!deleted)
            {
                throw new NotFoundException(nameof(Employee), id);
            }

            return true;
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
