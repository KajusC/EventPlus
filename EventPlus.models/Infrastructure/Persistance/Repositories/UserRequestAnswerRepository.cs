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
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");
            }
            return await _context.UserRequestAnswers
                .Include(ura => ura.FkQuestionidQuestionNavigation) // Įtraukia Question informaciją
                .Include(ura => ura.UserRequestAnswerUser) // Įtraukia UserRequestAnswerUser informaciją
                .Where(ura => ura.UserRequestAnswerUser != null && ura.UserRequestAnswerUser.FkUseridUser == userId)
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
            return await _dbSet
                .Include(ura => ura.UserRequestAnswerUser)
                .AnyAsync(ura => ura.UserRequestAnswerUser != null && ura.UserRequestAnswerUser.FkUseridUser == userId && ura.FkQuestionidQuestion == questionId);
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
        public async Task<bool> CreateBulkUserRequestAnswersWithUserAsync(List<UserRequestAnswer> userRequestAnswers, int userId)
        {
            if (userRequestAnswers == null || !userRequestAnswers.Any())
            {
                throw new ArgumentNullException(nameof(userRequestAnswers), "Atsakymų sąrašas negali būti tuščias.");
            }
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "Neteisingas vartotojo ID.");
            }

            // Pridedame atsakymus. EF Core automatiškai priskirs ID po AddRangeAsync,
            // bet mes juos gausime tik po SaveChangesAsync.
            // Todėl UserRequestAnswerUser kūrimą atliekame po pirminio išsaugojimo.

            await _dbSet.AddRangeAsync(userRequestAnswers);
            var saved = await _context.SaveChangesAsync() > 0;

            if (saved)
            {
                // Dabar, kai atsakymai turi ID, sukuriame UserRequestAnswerUser įrašus
                var userAnswersLinks = new List<UserRequestAnswerUser>();
                foreach (var answer in userRequestAnswers)
                {
                    // answer.IdUserRequestAnswer dabar turėtų turėti sugeneruotą reikšmę
                    if (answer.IdUserRequestAnswer > 0) // Patikrinam ar ID tikrai priskirtas
                    {
                        userAnswersLinks.Add(new UserRequestAnswerUser
                        {
                            FkUserRequestAnsweridUserRequestAnswer = answer.IdUserRequestAnswer,
                            FkUseridUser = userId
                        });
                    }
                }

                if (userAnswersLinks.Any())
                {
                    _context.UserRequestAnswerUsers.AddRange(userAnswersLinks);
                    return await _context.SaveChangesAsync() > 0;
                }
                return true; // Atsakymai išsaugoti, bet nebuvo ką susieti (neturėtų įvykti, jei answer.IdUserRequestAnswer visada > 0)
            }
            return false; // Nepavyko išsaugoti atsakymų
        }
    }
}