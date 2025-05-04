using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(EventPlusContext context) : base(context)
        {
        }
    }
}
