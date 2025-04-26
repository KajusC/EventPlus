using AutoMapper;
using eventplus.models.Domain.Events;
using EventPlus.Server.Application.Events.ViewModel;
using eventplus.models.Domain.Tickets;
using EventPlus.Server.Application.Events.ViewModel;
using EventPlus.Server.Application.Tickets.ViewModel;

namespace EventPlus.Server
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Event, EventViewModel>()
                .ForMember(dest => dest.FkEventLocationidEventLocation, opt => opt.MapFrom(src => src.FkEventLocationidEventLocation))
                .ForMember(dest => dest.FkOrganiseridUser, opt => opt.MapFrom(src => src.FkOrganiseridUser))
                .ReverseMap();

            CreateMap<Ticket, TicketViewModel>()
                .ForMember(dest => dest.SeatingId, opt => opt.MapFrom(src => src.FkSeatingidSeatingNavigation != null ? src.FkSeatingidSeatingNavigation.IdSeating : (int?)null))
                .ForMember(dest => dest.TicketStatusId, opt => opt.MapFrom(src => src.FkTicketstatusNavigation != null ? src.FkTicketstatusNavigation.IdStatus : (int?)null))
                .ReverseMap();
        }
    }
}
