using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services
{
    public class HotelsService(HotelListingDbContext context, ICountriesService countriesService, IMapper mapper) : IHotelsService
    {
        public async Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync()
        {
            //var hotels = await context.Hotels
            //    .Include(h => h.Country)
            //    .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            //    .ToListAsync();
            //return hotels;
            var hotels = await context.Hotels
                .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<GetHotelDto>>.Success(hotels);
        }

        public async Task<Result<GetHotelDto>> GetHotelAsync(int id)
        {
            //var hotel = await context.Hotels
            //.Where(h => h.HotelId == id)
            //.Include(h => h.Country)
            //.ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            //.FirstOrDefaultAsync();
            //return hotel ?? null;
            var hotel = await context.Hotels
                .Where(h => h.HotelId == id)
                .Include(h => h.Country)
                .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (hotel is null)
            {
                return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel with ID {id} not found."));
            }
            return Result<GetHotelDto>.Success(hotel);
        }

        public async Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hoteldto)
        {

            //var hotel = mapper.Map<Hotel>(createDto);
            //context.Hotels.Add(hotel);
            //await context.SaveChangesAsync();
            //var returnObj = mapper.Map<GetHotelDto>(hotel);
            //return returnObj;
            var countryExists = await countriesService.CountryExistsAsync(hoteldto.CountryId);
            if (!countryExists)
            {
                return Result<GetHotelDto>.Failure(new Error(ErrorCodes.BadRequest, $"Country with ID {hoteldto.CountryId} was not found."));
            }

            var duplicate = await HotelExistsAsync(hoteldto.Name, hoteldto.CountryId);
            if (duplicate)
            {
                return Result<GetHotelDto>.Failure(new Error(ErrorCodes.Conflict, $"A hotel with the name '{hoteldto.Name}' already exists in the specified country."));
            }

            var hotel = mapper.Map<Hotel>(hoteldto);
            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();

            var dto = await context.Hotels
                .Where(h => h.HotelId == hotel.HotelId)
                .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return Result<GetHotelDto>.Success(dto);
        }

        public async Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateDto)
        {
            //var hotel = await context.Hotels.FindAsync(id) ?? throw new KeyNotFoundException($"Hotel with ID {id} not found.");
            //hotel.Name = updateDto.Name;
            //hotel.Address = updateDto.Address;
            //hotel.Rating = updateDto.Rating;
            //hotel.CountryId = updateDto.CountryId;
            //context.Entry(hotel).State = EntityState.Modified;
            //context.Hotels.Update(hotel);
            //await context.SaveChangesAsync();
            if (id != updateDto.HotelId)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, "ID in the URL does not match ID in the request body."));
            }

            var hotel = await context.Hotels.FindAsync(id);
            if (hotel is null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel with ID {id} not found."));
            }

            var countryExists = await countriesService.CountryExistsAsync(updateDto.CountryId);
            if (!countryExists)
            {
                return Result.Failure(new Error(ErrorCodes.BadRequest, $"Country with ID {updateDto.CountryId} was not found."));
            }

            hotel.Name = updateDto.Name;
            hotel.Address = updateDto.Address;
            hotel.Rating = updateDto.Rating;
            hotel.CountryId = updateDto.CountryId;

            context.Hotels.Update(hotel);
            await context.SaveChangesAsync();
            return Result.Success();

        }

        public async Task<Result> DeleteHotelAsync(int id)
        {
            //var hotel = await context.Hotels
            //    .Where(h => h.HotelId == id)
            //    .ExecuteDeleteAsync();
            var affected = await context.Hotels
                .Where(h => h.HotelId == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel with ID {id} not found."));
            }

            return Result.Success();
        }
        public async Task<bool> HotelExistsAsync(string name, int id)
        {
            return await context.Hotels.AnyAsync(e => e.Name == name && e.HotelId == id);
        }
        public async Task<bool> HotelExists(string name)
        {
            return await context.Hotels.AnyAsync(e => e.Name == name);
        }
    }
}