namespace TicketSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Open";

        public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();
    }
}
