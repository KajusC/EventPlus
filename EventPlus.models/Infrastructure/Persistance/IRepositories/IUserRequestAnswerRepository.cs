using eventplus.models.Domain.UserAnswers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IUserRequestAnswerRepository : IRepository<UserRequestAnswer>
    {
        Task<List<UserRequestAnswer>> GetUserRequestAnswersByUserIdAsync(int userId);
        
        Task<bool> HasUserAnsweredQuestionAsync(int userId, int questionId); // Patikrina, ar vartotojas jau atsakė į konkretų klausimą

        Task<bool> CreateBulkUserRequestAnswersAsync(List<UserRequestAnswer> userRequestAnswers); // Masiniam atsakymų kūrimui
        
        // Task<bool> CreateUserRequestAnswerAsync(UserRequestAnswer UserRequestAnswer);
        // Task<bool> UpdateUserRequestAnswerAsync(UserRequestAnswer UserRequestAnswer);
        // Task<bool> DeleteUserRequestAnswerAsync(int id);

    }
}