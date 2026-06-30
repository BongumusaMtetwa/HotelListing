namespace HotelListing.Api.Data
{
    public class Hotel
    {
        public int HotelId { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public double Rating { get; set; }

        public decimal PerNightRate {get;set;}

        // Foreign key to the Country
        public int CountryId { get; set; }
        // Navigation property to the Country
        public Country? Country { get; set; }

        public ICollection<HotelAdmin>? HotelAdmins { get; set; } = [];

        public ICollection<Booking>? Bookings {get;set;} = [];
    }
}
