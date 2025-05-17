namespace EventPlus.Server.Application.ViewModels
{
    public class FeedbackViewModel
    {
        public int IdFeedback { get; set; }
        public string? Comment { get; set; }
        public decimal? Rating { get; set; }
        public int FkEventidEvent { get; set; }
        public int? Type { get; set; }
    }
}
