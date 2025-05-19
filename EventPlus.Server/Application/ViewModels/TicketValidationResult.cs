namespace EventPlus.Server.Application.ViewModels
{
    public class TicketValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public TicketViewModel? Ticket { get; set; }
    }
} 