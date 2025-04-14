﻿using eventplus.models.Entities;

namespace eventplus.models.Repository.FeedbackRepository
{
    public interface IFeedbackRepository
    {
        Task<bool> CreateFeedbackAsync(Feedback feedback);
        Task<bool> UpdateFeedbackAsync(Feedback feedback);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<Feedback> GetFeedbackByIdAsync(int id);
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<List<Feedback>> GetAllFeedbacksByEventIdAsync(int eventId);
        Task<bool> DeleteEventFeedbacks(int eventId);
    }
}
