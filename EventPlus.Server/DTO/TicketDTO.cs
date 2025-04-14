namespace EventPlus.Server.DTO
{
    public class TicketDTO
    {
        public int IdTicket { get; set; }

        public double? Price { get; set; }

        public DateOnly? GenerationDate { get; set; }

        public DateOnly? ScannedDate { get; set; }

        public string? QrCode { get; set; }

        public int? Type { get; set; }

        public int FkUseridUser { get; set; }

        public int FkEventidEvent { get; set; }

        public int? SeatingId { get; set; }

        public int? TicketStatusId { get; set; }
    }
}
