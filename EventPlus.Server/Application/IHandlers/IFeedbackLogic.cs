using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
    public interface IFeedbackLogic
    {
        Task<bool> CreateFeedbackAsync(FeedbackViewModel feedbackViewModel, int userId,string role);
        Task<bool> UpdateFeedbackAsync(FeedbackViewModel feedback);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<FeedbackViewModel> GetFeedbackByIdAsync(int id);
        Task<List<FeedbackViewModel>> GetAllFeedbacksAsync();

        Task<List<FeedbackViewModel>> GetFeedbacksByEventIdAsync(int eventId);
        Task<List<FeedbackViewModel>> GetFeedbacksByUserIdAsync(int userId);
        Task<bool> DeleteEventFeedbacks(int eventId);
    }
}
