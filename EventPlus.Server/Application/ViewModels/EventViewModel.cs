namespace EventPlus.Server.Application.ViewModels
{
    public class EventViewModel
    {
        public int IdEvent { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? MaxTicketCount { get; set; }
        public int? Category { get; set; }
        public int FkEventLocationidEventLocation { get; set; }
        public int FkOrganiseridUser { get; set; }
    }
}
