using eventplus.models.Domain.UserAnswers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAllAsync();
        Task<Question> GetByIdAsync(int id);
        Task<bool> CreateAsync(Question question);
        Task<bool> UpdateAsync(Question question);
        Task<bool> DeleteAsync(int id);
    }
}