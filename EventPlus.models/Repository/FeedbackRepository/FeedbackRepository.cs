using eventplus.models.context;
using eventplus.models.Entities;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Repository.FeedbackRepository
{
    public class FeedbackRepository : IFeedbackRepository
    {

        private readonly EventPlusContext _context;
        private readonly DbSet<Feedback> _feedbacks;

        public FeedbackRepository(EventPlusContext context)
        {
            _context = context;
            _feedbacks = context.Set<Feedback>();
        }

        public async Task<bool> CreateFeedbackAsync(Feedback feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            await _feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            var feedback = await _feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return false;
            }
            _feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventFeedbacks(int eventId)
        {
            var feedbacks = await _feedbacks.Where(f => f.FkEventidEvent == eventId).ToListAsync();
            if (feedbacks.Count == 0)
            {
                return false;
            }
            _feedbacks.RemoveRange(feedbacks);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _feedbacks
                .Include(f => f.FkEventidEventNavigation)
                .Include(f => f.FkUseridUserNavigation)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksByEventIdAsync(int eventId)
        {
            return await _feedbacks
                .Include(f => f.FkEventidEventNavigation)
                .Include(f => f.FkUseridUserNavigation)
                .Where(f => f.FkEventidEvent == eventId)
                .ToListAsync();
        }

        public async Task<Feedback> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbacks
                .Include(f => f.FkEventidEventNavigation)
                .Include(f => f.FkUseridUserNavigation)
                .FirstOrDefaultAsync(f => f.IdFeedback == id);

            if (feedback == null)
            {
                throw new KeyNotFoundException($"Feedback with ID {id} not found.");
            }

            return feedback;
        }

        public async Task<bool> UpdateFeedbackAsync(Feedback feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            var existingFeedback = await _feedbacks.FindAsync(feedback.IdFeedback);
            if (existingFeedback == null)
            {
                return false;
            }
            _context.Entry(existingFeedback).CurrentValues.SetValues(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
