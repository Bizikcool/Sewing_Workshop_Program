using SewingAPI.Service.DTOs.Employee;

namespace SewingAPI.Service.Mapping
{
    public static class EmployeeMapper
    {
        public static Domain.Entities.Employee ToEntity(EmployeeCreateDto dto)
        {
            return new Domain.Entities.Employee
            {
                Fio = dto.Fio,
                Position = dto.Position,
                Rank = dto.Rank
            };
        }

        public static Domain.Entities.Employee ToEntity(EmployeeUpdateDto dto)
        {
            return new Domain.Entities.Employee
            {
                Fio = dto.Fio,
                Position = dto.Position,
                Rank = dto.Rank
            };
        }

        public static EmployeeResponseDto ToResponse(Domain.Entities.Employee employee)
        {
            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Fio = employee.Fio,
                Position = employee.Position,
                Rank = employee.Rank,
                CreatedAt = employee.CreatedAt
            };
        }
    }
}
