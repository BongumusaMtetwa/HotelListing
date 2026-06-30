namespace HotelListing.Api.Data
{
    public class Country
    {
        
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        // Navigation property to the list of hotels in this country
        public IList<Hotel> Hotels { get; set; } = [];
    }
}
