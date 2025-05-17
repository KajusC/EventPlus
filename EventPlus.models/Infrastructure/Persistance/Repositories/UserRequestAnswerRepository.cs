using eventplus.models.Domain.UserAnswers;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class UserRequestAnswerRepository : Repository<UserRequestAnswer>, IUserRequestAnswerRepository
    {
        public UserRequestAnswerRepository(EventPlusContext context) : base(context)
        {
        }

        public async Task<List<UserRequestAnswer>> GetUserRequestAnswersByUserIdAsync(int userId)
        {
            return await _context.UserRequestAnswers
                .Include(u => u.FkQuestionidQuestionNavigation)
                .Include(u => u.UserRequestAnswerUser)
                .Where(u => u.UserRequestAnswerUser != null && u.UserRequestAnswerUser.FkUseridUser == userId)
                .ToListAsync();
        }

        public override async Task<List<UserRequestAnswer>> GetAllAsync()
        {
            return await _context.UserRequestAnswers
                .Include(u => u.FkQuestionidQuestionNavigation)
                .Include(u => u.UserRequestAnswerUser)
                    .ThenInclude(uru => uru != null ? uru.FkUseridUserNavigation : null)
                .Include(u => u.UserRequestAnswerAdministrator)
                .Include(u => u.UserRequestAnswerOrganiser)
                .ToListAsync();
        }

        public override async Task<UserRequestAnswer> GetByIdAsync(int id)
        {
            return await _context.UserRequestAnswers
                .Include(u => u.FkQuestionidQuestionNavigation)
                .Include(u => u.UserRequestAnswerUser)
                    .ThenInclude(uru => uru.FkUseridUserNavigation)
                .Include(u => u.UserRequestAnswerAdministrator)
                    .ThenInclude(ura => ura.FkAdministratoridUserNavigation)
                .Include(u => u.UserRequestAnswerOrganiser)
                    .ThenInclude(uro => uro.FkOrganiseridUserNavigation)
                .FirstOrDefaultAsync(u => u.IdUserRequestAnswer == id);
        }
        public override async Task<bool> CreateAsync(UserRequestAnswer UserRequestAnswer)
        {
            if (UserRequestAnswer == null)
            {
                throw new ArgumentNullException(nameof(UserRequestAnswer));
            }

            _dbSet.Add(UserRequestAnswer);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HasUserAnsweredQuestionAsync(int userId, int questionId)
        {
            return await _dbSet.AnyAsync(ura => ura.UserRequestAnswerUser.FkUseridUser == userId && ura.FkQuestionidQuestionNavigation.IdQuestion == questionId);
        }
        public async Task<bool> CreateBulkUserRequestAnswersAsync(List<UserRequestAnswer> userRequestAnswers)
        {
            if (userRequestAnswers == null || !userRequestAnswers.Any())
            {
                throw new ArgumentNullException(nameof(userRequestAnswers), "Atsakymų sąrašas negali būti tuščias.");
            }

            await _dbSet.AddRangeAsync(userRequestAnswers);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}