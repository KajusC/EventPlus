using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.context;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public interface ITicketStatusRepository : IRepository<TicketStatus>
    {
    }

    public class TicketStatusRepository : Repository<TicketStatus>, ITicketStatusRepository
    {
        public TicketStatusRepository(EventPlusContext context) : base(context)
        {
        }
    }

}
