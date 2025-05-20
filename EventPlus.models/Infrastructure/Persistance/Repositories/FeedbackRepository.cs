using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
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

        public async Task<bool> CreateFeedbackAsync(Feedback feedback, int userId, string role)
        {
            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback));

            await _feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();

            if (role == "User")
            {
                var userFeedback = new UserFeedback
                {
                    FkUseridUser = userId,
                    FkFeedbackidFeedback = feedback.IdFeedback
                };
                await _context.UserFeedbacks.AddAsync(userFeedback);
            }
            else if (role == "Administrator")
            {
                var adminFeedback = new AdministratorFeedback
                {
                    FkAdministratoridUser = userId,
                    FkFeedbackidFeedback = feedback.IdFeedback
                };
                await _context.AdministratorFeedbacks.AddAsync(adminFeedback);
            }
            else if (role == "Organiser")
            {
                var organiserFeedback = new OrganiserFeedback
                {
                    FkOrganiseridUser = userId,
                    FkFeedbackidFeedback = feedback.IdFeedback
                };
                await _context.OrganiserFeedbacks.AddAsync(organiserFeedback);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            var feedback = await _feedbacks.FindAsync(id);
            if (feedback == null)
                return false;

            var userFeedback = await _context.UserFeedbacks.FirstOrDefaultAsync(uf => uf.FkFeedbackidFeedback == id);
            if (userFeedback != null)
                _context.UserFeedbacks.Remove(userFeedback);

            var adminFeedback = await _context.AdministratorFeedbacks.FirstOrDefaultAsync(af => af.FkFeedbackidFeedback == id);
            if (adminFeedback != null)
                _context.AdministratorFeedbacks.Remove(adminFeedback);

            var organiserFeedback = await _context.OrganiserFeedbacks.FirstOrDefaultAsync(of => of.FkFeedbackidFeedback == id);
            if (organiserFeedback != null)
                _context.OrganiserFeedbacks.Remove(organiserFeedback);

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
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksByEventIdAsync(int eventId)
        {
            return await _feedbacks
                .Include(f => f.FkEventidEventNavigation)
                //.Include(f => f.FkUseridUserNavigation)
                .Where(f => f.FkEventidEvent == eventId)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksByUserIdAsync(int userId)
        {
            // Get all feedback IDs associated with this user
            var userFeedbackIds = await _context.UserFeedbacks
                .Where(uf => uf.FkUseridUser == userId)
                .Select(uf => uf.FkFeedbackidFeedback)
                .ToListAsync();
                
            var adminFeedbackIds = await _context.AdministratorFeedbacks
                .Where(af => af.FkAdministratoridUser == userId)
                .Select(af => af.FkFeedbackidFeedback)
                .ToListAsync();
                
            var organiserFeedbackIds = await _context.OrganiserFeedbacks
                .Where(of => of.FkOrganiseridUser == userId)
                .Select(of => of.FkFeedbackidFeedback)
                .ToListAsync();
                
            // Combine all feedback IDs
            var allFeedbackIds = userFeedbackIds
                .Concat(adminFeedbackIds)
                .Concat(organiserFeedbackIds)
                .Distinct()
                .ToList();
                
            // Get the actual feedback objects
            return await _feedbacks
                .Include(f => f.FkEventidEventNavigation)
                .Where(f => allFeedbackIds.Contains(f.IdFeedback))
                .ToListAsync();
        }

        public async Task<Feedback> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbacks
                .Include(f => f.FkEventidEventNavigation)
                //.Include(f => f.FkUseridUserNavigation)
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

        public async Task<List<Feedback>> GetFeedbacksByLocationIdAsync(int locationId)
        {
            var eventIdsInLocation = await _context.Events
                                              .Where(e => e.FkEventLocationidEventLocation == locationId)
                                              .Select(e => e.IdEvent)
                                              .ToListAsync();

            if (!eventIdsInLocation.Any())
            {
                return new List<Feedback>();
            }

            return await _feedbacks
                               .Where(f => eventIdsInLocation.Contains(f.FkEventidEvent))
                               .Include(f => f.FkEventidEventNavigation)
                               .ToListAsync();
        }
    }
}
