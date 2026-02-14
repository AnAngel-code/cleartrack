namespace TicketSystem.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public string Category { get; set; }

        public string Status {get; set;}
        public string Note { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
