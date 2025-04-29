using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class PerformerRepository : Repository<Performer>
    {
        public PerformerRepository(EventPlusContext context) : base(context)
        {
        }
    }
}
