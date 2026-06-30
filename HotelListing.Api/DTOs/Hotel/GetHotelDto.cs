namespace HotelListing.Api.DTOs.Hotel;

public record GetHotelDto(
    int HotelId,
    string Name,
    string Address,
    double Rating,
    int CountryId,
    string Country
);

//public record GetHotelDto(
//    int HotelId,
//    string Name,
//    string Address,
//    double Rating,
//    int CountryId
//);

public record GetHotelSlimDto(
    int HotelId,
    string Name,
    string Address,
    double Rating
);


