using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Hotel;

// This DTO (Data Transfer Object) class is used to represent the data needed to create a new hotel. It includes properties for the hotel's name, address, rating, and the ID of the country it belongs to.
// The [Required] attribute indicates that these properties must be provided when creating a hotel, while the [MaxLength] and [Range] attributes enforce constraints on the address and rating properties, respectively.
public class CreateHotelDto
{
    [Required]
    public required string Name { get; set; }
    
    [MaxLength(150)]
    public required string Address { get; set; }
    
    [Range(1, 5)]
    public double Rating { get; set; }
    
    public required int CountryId { get; set; }
}
