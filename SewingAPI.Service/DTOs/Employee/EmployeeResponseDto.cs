namespace SewingAPI.Service.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
