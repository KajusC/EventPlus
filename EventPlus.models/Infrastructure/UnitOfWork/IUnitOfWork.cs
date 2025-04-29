using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.Persistance;
using eventplus.models.Infrastructure.Persistance.IRepositories;


namespace eventplus.models.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IEventRepository Events { get; }
        IRepository<EventLocation> EventLocations { get; }
        IRepository<Partner> Partners { get; }
        IRepository<Equipment> Equipments { get; }
        IRepository<Performer> Performers { get; }
        ISectorRepository Sectors { get; }
        IFeedbackRepository Feedbacks { get; }
        ITicketRepository Tickets { get; }
        Task SaveAsync();
    }
}
