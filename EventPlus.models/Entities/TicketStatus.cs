namespace eventplus.models.Entities
{
    public class TicketStatus
    {
        public int IdTicketStatus { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
