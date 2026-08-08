namespace SewingAPI.Service.DTOs.Employee
{
    public class EmployeeCreateDto
    {
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
    }
}
