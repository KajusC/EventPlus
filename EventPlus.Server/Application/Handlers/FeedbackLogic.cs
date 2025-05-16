using AutoMapper;
using eventplus.models.Domain.Feedbacks;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using System.Data;

namespace EventPlus.Server.Application.Handlers
{
    public class FeedbackLogic : IFeedbackLogic
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        public FeedbackLogic(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }
        public async Task<bool> CreateFeedbackAsync(FeedbackViewModel feedback, int userId, string role)
        {
            if (string.IsNullOrWhiteSpace(feedback.Comment))
            {
                throw new ArgumentException("Comment cannot be empty.");
            }

            if (feedback.Rating < 1 || feedback.Rating > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(feedback.Rating), "Rating must be between 1 and 10.");
            }

            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            var feedbackEntity = _mapper.Map<Feedback>(feedback);
            return await _feedbackRepository.CreateFeedbackAsync(feedbackEntity, userId, role);
        }

        public Task<bool> DeleteEventFeedbacks(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }
            return _feedbackRepository.DeleteEventFeedbacks(eventId);
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _feedbackRepository.DeleteFeedbackAsync(id);
        }

        public async Task<List<FeedbackViewModel>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
            return _mapper.Map<List<FeedbackViewModel>>(feedbacks);
        }

        public async Task<FeedbackViewModel> GetFeedbackByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var feedbackEntity = await _feedbackRepository.GetFeedbackByIdAsync(id);
            return _mapper.Map<FeedbackViewModel>(feedbackEntity);
        }
        public async Task<List<FeedbackViewModel>> GetFeedbacksByEventIdAsync(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }

            var feedbacks = await _feedbackRepository.GetAllFeedbacksByEventIdAsync(eventId);
            return _mapper.Map<List<FeedbackViewModel>>(feedbacks);
        }

        public async Task<bool> UpdateFeedbackAsync(FeedbackViewModel feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            var feedbackEntity = _mapper.Map<Feedback>(feedback);
            return await _feedbackRepository.UpdateFeedbackAsync(feedbackEntity);
        }
    }
}
