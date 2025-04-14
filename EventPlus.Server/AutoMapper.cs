using AutoMapper;
using eventplus.models.Entities;
using EventPlus.Server.DTO;

namespace EventPlus.Server
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Event, EventDTO>()
                .ForMember(dest => dest.FkEventLocationidEventLocation, opt => opt.MapFrom(src => src.FkEventLocationidEventLocation))
                .ForMember(dest => dest.FkOrganiseridUser, opt => opt.MapFrom(src => src.FkOrganiseridUser))
                .ReverseMap();

            CreateMap<Ticket, TicketDTO>()
                .ForMember(dest => dest.SeatingId, opt => opt.MapFrom(src => src.Seating != null ? src.Seating.IdSeating : (int?)null))
                .ForMember(dest => dest.TicketStatusId, opt => opt.MapFrom(src => src.TicketStatuses != null ? src.TicketStatuses.IdTicketStatus : (int?)null))
                .ReverseMap();
        }
    }
}
