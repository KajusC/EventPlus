using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Sectors;
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

            CreateMap<Category, CategoryViewModel>()
                .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.IdCategory))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<SectorViewModel, Sector>()
                .ForMember(dest => dest.IdSector, opt => opt.MapFrom(src => src.IdSector))
                .ForMember(dest => dest.FkEventLocationidEventLocation, opt => opt.MapFrom(src => src.FkEventLocationidEventLocation))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

			CreateMap<SectorPriceViewModel, SectorPrice>()
                .ForMember(dest => dest.IdSectorPrice, opt => opt.Ignore())
	            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
	            .ForMember(dest => dest.FkSectoridSector, opt => opt.MapFrom(src => src.SectorId))
				.ForMember(dest => dest.FkEventidEvent, opt => opt.MapFrom(src => src.EventId))
				.ReverseMap();

			CreateMap<SeatingViewModel, Seating>()
	            .ForMember(dest => dest.IdSeating, opt => opt.MapFrom(src => src.IdSeating))
	            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.Row))
	            .ForMember(dest => dest.FkSectoridSector, opt => opt.MapFrom(src => src.SectorId))
	            .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
	            .ReverseMap();
            CreateMap<Feedback, FeedbackViewModel>()
                .ForMember(dest => dest.IdFeedback, opt => opt.MapFrom(src => src.IdFeedback))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.FkEventidEvent, opt => opt.MapFrom(src => src.FkEventidEvent))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ReverseMap();
        }
    }
}
