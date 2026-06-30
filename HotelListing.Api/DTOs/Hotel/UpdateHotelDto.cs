using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Hotel;

// This DTO (Data Transfer Object) class is used to represent the data needed to update an existing hotel. It inherits from the CreateHotelDto class, which means it includes all the properties required to create a hotel (Name, Address, Rating, CountryId) and adds an additional property, HotelId, which is required to identify the hotel being updated.
// The [Required] attribute ensures that the HotelId must be provided when updating a hotel.
public class UpdateHotelDto: CreateHotelDto
{
    [Required]
    public int HotelId { get; set; }

}
