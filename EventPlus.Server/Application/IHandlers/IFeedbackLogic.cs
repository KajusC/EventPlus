using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
    public interface IFeedbackLogic
    {
        Task<bool> CreateFeedbackAsync(FeedbackViewModel feedback);
        Task<bool> UpdateFeedbackAsync(FeedbackViewModel feedback);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<FeedbackViewModel> GetFeedbackByIdAsync(int id);
        Task<List<FeedbackViewModel>> GetAllFeedbacksAsync();

        Task<bool> DeleteEventFeedbacks(int eventId);
    }
}
