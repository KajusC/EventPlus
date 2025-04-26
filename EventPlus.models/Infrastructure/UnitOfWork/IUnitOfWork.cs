using eventplus.models.Infrastructure.Persistance.Repositories.Events;
using eventplus.models.Infrastructure.Persistance.Repositories.Feedbacks;
using eventplus.models.Infrastructure.Persistance.Repositories.Sectors;
using eventplus.models.Infrastructure.Persistance.Repositories.Tickets;


namespace eventplus.models.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IEventRepository Events { get; }
        ISectorRepository Sectors { get; }
        IFeedbackRepository Feedbacks { get; }
        ITicketRepository Tickets { get; }
        Task SaveAsync();
    }
}
