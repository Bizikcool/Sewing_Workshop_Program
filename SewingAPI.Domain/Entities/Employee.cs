namespace SewingAPI.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
