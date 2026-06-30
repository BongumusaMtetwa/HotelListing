using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Data;
using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Country;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context, IMapper mapper) : ICountriesService
{
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        var countries = await context.Countries
        .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
        .ToListAsync();

        return Result<IEnumerable<GetCountriesDto>>.Success(countries);

    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        var country = await context.Countries
            .Where(c => c.CountryId == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return country is null
            ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country with ID {id} not found."))
            : Result<GetCountryDto>.Success(country);
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto createDto)
    {
        try
        {
            var exists = await CountryExistsAsync(createDto.Name);
            if (exists)
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, $"A country with the name '{createDto.Name}' already exists."));
            }

            var country = mapper.Map<Country>(createDto);
            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var dto = await context.Countries
                .Where(c => c.CountryId == country.CountryId)
                .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return Result<GetCountryDto>.Success(dto);
        }
        catch
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Failure, "An unexpected error occurred while creating the country."));
        }
    }

    public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto)
    {
        try
        {
            if (id != updateDto.CountryId)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, "ID in the URL does not match ID in the request body."));
            }
            var country = await context.Countries.FindAsync(id);
            if (country == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with ID {id} not found."));
            }
            country.Name = updateDto.Name;
            country.ShortName = updateDto.ShortName;
            context.Countries.Update(country);
            await context.SaveChangesAsync();

            return Result.Success();
        }
        catch
        {
            return Result.Failure(new Error("Failure", "An unexpected error occurred while updating the country."));
        }

    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await context.Countries.FindAsync(id);
            if (country == null)
            {
                return Result.NotFound(new Error("NotFound", $"Country with ID {id} not found."));
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();

            return Result.Success();
        }

        catch
        {
            return Result.Failure(new Error(ErrorCodes.Failure, "An unexpected error occurred while deleting the country."));
        }

    }
    // Helper method to check if a country exists by ID
    public async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.CountryId == id);
    }
    // Helper method to check if a country exists by name (ignoring case and whitespace)
    public async Task<bool> CountryExistsAsync(string name)
    {
        return await context.Countries.AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
    }
}
