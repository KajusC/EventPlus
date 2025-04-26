namespace EventPlus.Server.Application.Events.ViewModel
{
    public class EventViewModel
    {
        public int IdEvent { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxTicketCount { get; set; }
        public int? Category { get; set; }
        public int FkEventLocationidEventLocation { get; set; }
        public int FkOrganiseridUser { get; set; }
    }
}
