using Microsoft.EntityFrameworkCore;
using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Repo.Data;

namespace SewingAPI.Repo.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _db;

        public EmployeeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<Employee> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, string? position, int? rank)
        {
            var query = _db.Employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(e => e.Fio.ToLower().Contains(term) || e.Position.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(position))
            {
                var normalizedPosition = position.Trim().ToLower();
                query = query.Where(e => e.Position.ToLower().Contains(normalizedPosition));
            }

            if (rank.HasValue)
            {
                query = query.Where(e => e.Rank == rank.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.Fio)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<Employee?> GetByIdAsync(int id)
        {
            return _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee is null)
            {
                return false;
            }

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
