using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.Persistance;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.Persistance.Repositories;

namespace eventplus.models.Infrastructure.UnitOfWork
{
public interface IUnitOfWork
    {
        IOrganiserRepository Organisers { get; }
        IEventRepository Events { get; }
        IRepository<EventLocation> EventLocations { get; }
        IRepository<Partner> Partners { get; }
        IRepository<Equipment> Equipments { get; }
        IRepository<Performer> Performers { get; }
        ISectorRepository Sectors { get; }
        IFeedbackRepository Feedbacks { get; }
        ITicketRepository Tickets { get; }
        ICategoryRepository Categories { get; }
        ISectorPriceRepository SectorPrices { get; }
        ISeatingRepository Seatings { get; }
        ITicketStatusRepository TicketStatus { get; }
        IUserRequestAnswerRepository UserRequestAnswers { get; }
		    IQuestionRepository Questions { get; }
        Task SaveAsync();
    }
}