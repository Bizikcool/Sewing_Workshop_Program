using SewingAPI.Domain.Exceptions;

namespace SewingAPI.Service.Validation
{
    public static class EmployeeValidator
    {
        public static void Validate(Domain.Entities.Employee employee)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(employee.Fio))
            {
                errors.Add("ФИО сотрудника обязательно.");
            }

            if (string.IsNullOrWhiteSpace(employee.Position))
            {
                errors.Add("Должность обязательна.");
            }

            if (employee.Rank < 0)
            {
                errors.Add("Разряд не может быть отрицательным.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
