using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Country;

// This DTO (Data Transfer Object) class is used to represent the data needed to update an existing country. It inherits from the CreateCountryDto class,
// which means it includes all the properties required to create a country (Name and ShortName) and adds an additional property, CountryId, which is required to identify the country being updated.
public class  UpdateCountryDto : CreateCountryDto
{
    [Required]
    public int CountryId { get; set; }
    
}
