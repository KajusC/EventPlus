using eventplus.models.Domain.UserAnswers;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(EventPlusContext context) : base(context)
        {
        }

        public override async Task<List<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.FkAdministratoridUserNavigation)
                .ToListAsync();
        }

        public override async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.FkAdministratoridUserNavigation)
                .FirstOrDefaultAsync(q => q.IdQuestion == id);
        }
        public override async Task<bool> CreateAsync(Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            _dbSet.Add(question);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}