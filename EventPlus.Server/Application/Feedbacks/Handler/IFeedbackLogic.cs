using EventPlus.Server.Application.Feedbacks.ViewModel;

namespace EventPlus.Server.Application.Feedbacks.Handler
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
