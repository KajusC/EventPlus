using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Sectors;
using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Domain.Users;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Event, EventViewModel>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.HasValue ? src.StartDate.Value : (DateOnly?)null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value : (DateOnly?)null))
                .ForMember(dest => dest.FkEventLocationidEventLocation, opt => opt.MapFrom(src => src.FkEventLocationidEventLocation))
                .ForMember(dest => dest.FkOrganiseridUser, opt => opt.MapFrom(src => src.FkOrganiseridUser))
                .ReverseMap();

            CreateMap<Ticket, TicketViewModel>()
				.ForMember(dest => dest.IdTicket, opt => opt.MapFrom(src => src.IdTicket))
				.ForMember(dest => dest.SeatingId, opt => opt.MapFrom(src => src.FkSeatingidSeating))
				.ForMember(dest => dest.TicketStatusId, opt => opt.MapFrom(src => src.FkTicketstatus))
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

			CreateMap<OrganiserViewModel, Organiser>()
	            .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.IdUser))
	            .ForMember(dest => dest.FollowerCount, opt => opt.MapFrom(src => src.FollowerCount))
	            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
	            .ReverseMap();
            CreateMap<UserRequestAnswer, UserRequestAnswerViewModel>()
                .ForMember(dest => dest.IdUserRequestAnswer, opt => opt.MapFrom(src => src.IdUserRequestAnswer))
                .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer))
                .ForMember(dest => dest.FkQuestionidQuestion, opt => opt.MapFrom(src => src.FkQuestionidQuestion))
                .ReverseMap();
            CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.IdQuestion, opt => opt.MapFrom(src => src.IdQuestion))
                .ForMember(dest => dest.FormulatedQuestion, opt => opt.MapFrom(src => src.FormulatedQuestion))
                .ForMember(dest => dest.FkAdministratoridUser, opt => opt.MapFrom(src => src.FkAdministratoridUser))
                .ForMember(dest => dest.AdministratorName, opt => opt.MapFrom(src => src.FkAdministratoridUserNavigation != null ? $"{src.FkAdministratoridUserNavigation.Name} {src.FkAdministratoridUserNavigation.Surname}" : null))
                .ReverseMap();

		}
    }
}
