using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Domain.Tickets;
using EventPlus.Server.Application.ViewModels;

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

            CreateMap<EventLocation, EventLocationViewModel>()
                .ForMember(dest => dest.IdEventLocation, opt => opt.MapFrom(src => src.IdEventLocation))
                .ForMember(dest => dest.IDEquipment, opt => opt.MapFrom(src => src.HoldingEquipmentNavigation != null ? src.HoldingEquipmentNavigation.IdEquipment : (int?)null))
                .ForMember(dest => dest.SectorIds, opt => opt.MapFrom(src => src.Sectors.Select(s => s.IdSector).ToList()))
                .ReverseMap();

            CreateMap<Partner, PartnerViewModel>()
                .ReverseMap();

            CreateMap<Performer, PerformerViewModel>()
                .ForMember(dest => dest.IdPerformer, opt => opt.MapFrom(src => src.IdPerformer))
                .ReverseMap();

        }
    }
}
