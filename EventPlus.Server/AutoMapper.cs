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
        }
    }
}
