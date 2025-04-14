using AutoMapper;
using eventplus.models.Repository.FeedbackRepository;
using EventPlus.Server.DTO;
using EventPlus.Server.Logic.Interface;

namespace EventPlus.Server.Logic
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
        public async Task<bool> CreateFeedbackAsync(FeedbackDTO feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            var feedbackEntity = _mapper.Map<eventplus.models.Entities.Feedback>(feedback);
            return await _feedbackRepository.CreateFeedbackAsync(feedbackEntity);
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

        public async Task<List<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
            return _mapper.Map<List<FeedbackDTO>>(feedbacks);
        }

        public async Task<FeedbackDTO> GetFeedbackByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var feedbackEntity = await _feedbackRepository.GetFeedbackByIdAsync(id);
            return _mapper.Map<FeedbackDTO>(feedbackEntity);
        }

        public async Task<bool> UpdateFeedbackAsync(FeedbackDTO feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            var feedbackEntity = _mapper.Map<eventplus.models.Entities.Feedback>(feedback);
            return await _feedbackRepository.UpdateFeedbackAsync(feedbackEntity);
        }
    }
}
