using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.Persistance.Repositories;

namespace eventplus.models.Infrastructure.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly EventPlusContext _context;
		private IEventRepository? _events;
		private IRepository<EventLocation>? _eventLocations;
		private IRepository<Partner>? _partners;
		private IRepository<Equipment>? _equipments;
		private IRepository<Performer>? _performers;
		private ISectorRepository? _sectors;
		private IFeedbackRepository? _feedbacks;
		private ITicketRepository? _tickets;
		private ICategoryRepository? _categories;
		private ISectorPriceRepository? _sectorPrices;
		private ISeatingRepository? _seatings;
		private IOrganiserRepository? _organisers;
		private IUserRequestAnswerRepository? _UserRequestAnswersAnswers;
		private IQuestionRepository _questions;
    private ITicketStatusRepository? _ticketStatus;

        public UnitOfWork(EventPlusContext context)
        {
            _context = context;
        }

		public IEventRepository Events => _events ??= new EventRepository(_context);
		public IRepository<EventLocation> EventLocations => _eventLocations ??= new Repository<EventLocation>(_context);
		public IRepository<Partner> Partners => _partners ??= new Repository<Partner>(_context);
		public IRepository<Equipment> Equipments => _equipments ??= new Repository<Equipment>(_context);
		public IRepository<Performer> Performers => _performers ??= new Repository<Performer>(_context);
		public ISectorRepository Sectors => _sectors ??= new SectorRepository(_context);
		public IFeedbackRepository Feedbacks => _feedbacks ??= new FeedbackRepository(_context);
		public ITicketRepository Tickets => _tickets ??= new TicketRepository(_context);
		public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
		public ISectorPriceRepository SectorPrices => _sectorPrices ??= new SectorPriceRepository(_context);
		public ISeatingRepository Seatings => _seatings ??= new SeatingRepository(_context);
		public IOrganiserRepository Organisers => _organisers ??= new OrganiserRepository(_context);
		public IUserRequestAnswerRepository UserRequestAnswers => _UserRequestAnswersAnswers ??= new UserRequestAnswerRepository(_context);
		public IQuestionRepository Questions => _questions ??= new QuestionRepository(_context);
    public IOrganiserRepository Organisers => _organisers ??= new OrganiserRepository(_context);


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}