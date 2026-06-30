using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Country;

// This DTO (Data Transfer Object) class is used to represent the data needed to create a new country.
// It includes properties for the country's name and short name, both of which are required and have maximum length constraints defined by the [Required] and [MaxLength] attributes, respectively.
public class CreateCountryDto
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(3)]
    public required string ShortName { get; set; }

}
