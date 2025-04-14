using EventPlus.Server.DTO;

namespace EventPlus.Server.Logic.Interface
{
    public interface IFeedbackLogic
    {
        Task<bool> CreateFeedbackAsync(FeedbackDTO feedback);
        Task<bool> UpdateFeedbackAsync(FeedbackDTO feedback);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<FeedbackDTO> GetFeedbackByIdAsync(int id);
        Task<List<FeedbackDTO>> GetAllFeedbacksAsync();

        Task<bool> DeleteEventFeedbacks(int eventId);
    }
}
