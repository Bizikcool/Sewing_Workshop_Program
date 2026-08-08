namespace SewingAPI.Domain.Entities
{
    public class Fill
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Related { get; set; } = string.Empty;
        public string Status { get; set; } = "Выполнено";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
