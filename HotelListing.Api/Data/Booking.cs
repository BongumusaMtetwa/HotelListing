namespace HotelListing.Api.Data;

public class Booking
{
    public int BookingId { get; set; }

    public required int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public required DateOnly CheckInDate { get; set; }
    public required DateOnly CheckOutDate { get; set; }

    public int Guests { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
}
