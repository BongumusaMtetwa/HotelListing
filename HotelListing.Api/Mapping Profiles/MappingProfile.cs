using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.Mapping_Profiles;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        // Mapping configuration for Country entity and DTOs
        CreateMap<Hotel, GetHotelDto>()
            .ForCtorParam("Country", opt => opt.MapFrom(src => 
        src.Country != null ? src.Country.Name : string.Empty));
        CreateMap<Hotel, GetHotelSlimDto>();
        CreateMap<CreateHotelDto, Hotel>();
    }
}

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        // Mapping configuration for Country entity and DTOs
        CreateMap<Country, GetCountryDto>()
            .ForMember(d => d.CountryId, cfg => cfg.MapFrom(s => s.CountryId));
        CreateMap<Country, GetCountriesDto>()
            .ForMember(d => d.CountryId, cfg => cfg.MapFrom(s => s.CountryId));
        CreateMap<CreateCountryDto, Country>();
    }
}

//Resolver to resolve the Country name when mapping from Hotel to GetHotelDto
public class CountryNameResolver : IValueResolver<Hotel, GetHotelDto, string>
{
    public string Resolve(Hotel source, GetHotelDto destination, string destMember, ResolutionContext context)
    {
        return source.Country?.Name ?? string.Empty;
    }
}


public sealed class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, GetBookingDto>()
            .ForMember(d => d.HotelName, o => o.MapFrom(s=> s.Hotel!.Name))
            .ForMember(d => d.Status, o=>o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateBookingDto, Booking>()
            .ForMember(d => d.BookingId, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.TotalPrice, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Hotel, o => o.Ignore());

        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(d => d.BookingId, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.TotalPrice, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Hotel, o => o.Ignore());
    }
}
