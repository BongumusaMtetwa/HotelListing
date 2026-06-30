using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.DTOs.Country;

public class GetCountryDto
{
    public int CountryId { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public List<GetHotelSlimDto>? Hotels { get; set; } = new();
}
    
public class GetCountriesDto
{
    public int CountryId { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }  
}
