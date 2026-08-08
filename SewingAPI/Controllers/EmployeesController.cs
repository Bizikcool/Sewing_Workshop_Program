using Microsoft.AspNetCore.Mvc;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.DTOs.Employee;
using SewingAPI.Service.DTOs.Shared;
using SewingAPI.Service.Mapping;

namespace SewingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employees;

        public EmployeesController(IEmployeeService employees)
        {
            _employees = employees;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<EmployeeResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? position = null,
            [FromQuery] int? rank = null)
        {
            var normalizedPage = NormalizePage(page);
            var normalizedPageSize = NormalizePageSize(pageSize);
            var (items, total) = await _employees.GetPagedAsync(normalizedPage, normalizedPageSize, search, position, rank);

            return Ok(new PagedResult<EmployeeResponseDto>
            {
                Items = items.Select(EmployeeMapper.ToResponse).ToList(),
                Total = total,
                Page = normalizedPage,
                PageSize = normalizedPageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetById(int id)
        {
            var employee = await _employees.GetByIdAsync(id);
            return Ok(EmployeeMapper.ToResponse(employee));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> Create(EmployeeCreateDto dto)
        {
            var employee = await _employees.CreateAsync(EmployeeMapper.ToEntity(dto));
            var response = EmployeeMapper.ToResponse(employee);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<EmployeeResponseDto>> Update(int id, EmployeeUpdateDto dto)
        {
            var employee = await _employees.UpdateAsync(id, EmployeeMapper.ToEntity(dto));
            return Ok(EmployeeMapper.ToResponse(employee));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employees.DeleteAsync(id);
            return NoContent();
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
