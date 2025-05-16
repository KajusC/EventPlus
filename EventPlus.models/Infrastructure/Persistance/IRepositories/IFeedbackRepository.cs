using eventplus.models.Domain.Feedbacks;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IFeedbackRepository
    {
        Task<bool> CreateFeedbackAsync(Feedback feedback, int userId,string role);
        Task<bool> UpdateFeedbackAsync(Feedback feedback);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<Feedback> GetFeedbackByIdAsync(int id);
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<List<Feedback>> GetAllFeedbacksByEventIdAsync(int eventId);
        Task<bool> DeleteEventFeedbacks(int eventId);
    }
}
