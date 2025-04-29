using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class PartnerRepository : Repository<Partner>
    {
        public PartnerRepository(EventPlusContext context) : base(context)
        {
        }
    }
}
