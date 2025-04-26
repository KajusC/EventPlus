namespace EventPlus.Server.Application.Feedbacks.ViewModel
{
    public class FeedbackViewModel
    {
        public int IdFeedback { get; set; }
        public string? Comment { get; set; }
        public int? Rating { get; set; }
        public int FkEventidEvent { get; set; }
        public int FkUseridUser { get; set; }
        public int? IdTicketType { get; set; }
    }
}
