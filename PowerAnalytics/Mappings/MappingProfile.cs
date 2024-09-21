using AutoMapper;
using PowerAnalytics.DTOs;
using PowerAnalytics.Models;

namespace PowerAnalytics.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PowerReading, PowerReadingDto>().ReverseMap();
    }
}
