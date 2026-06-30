using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Booking;

public record GetBookingDto(
    int BookingId,
    int HotelId,
    string HotelName,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int Guests,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateBookingDto(
    [Required] int HotelId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    [Required][Range(minimum: 1, maximum: 10)] int Guests
) : IValidatableObject

{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "Check-out date must be after check-in date.",
                [nameof(CheckOutDate), nameof(CheckInDate)]
            );
        }
    }
}