using HotelListing.Api.Common.Results;
using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.Services
{
    public interface IHotelsService
    {
        Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hoteldto);
        Task<Result> DeleteHotelAsync(int id);
        Task<Result<GetHotelDto>> GetHotelAsync(int id);
        Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync();
        Task<bool> HotelExists(string name);
        Task<bool> HotelExistsAsync(string name, int id);
        Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    }
}