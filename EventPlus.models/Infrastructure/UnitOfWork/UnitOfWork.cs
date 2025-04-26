using eventplus.models.Infrastructure.Persistance.Repositories.Events;
using eventplus.models.Infrastructure.Persistance.Repositories.Feedbacks;
using eventplus.models.Infrastructure.Persistance.Repositories.Sectors;
using eventplus.models.Infrastructure.Persistance.Repositories.Tickets;

namespace eventplus.models.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IEventRepository Events => throw new NotImplementedException();

        public ISectorRepository Sectors => throw new NotImplementedException();

        public IFeedbackRepository Feedbacks => throw new NotImplementedException();

        public ITicketRepository Tickets => throw new NotImplementedException();

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}

