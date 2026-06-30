using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Booking;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;



public class BookingService(HotelListingDbContext context, IUsersService usersService, IMapper mapper) : IBookingService
{
    public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId)
    {
        var hotelExists = await context.Hotels.AnyAsync(h => h.HotelId == hotelId);
        if (!hotelExists)
        {
            return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{hotelId}' was not found."));
        }

        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId)
            .OrderBy(b => b.CheckInDate)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider) //Use projection from Mapper configuration instead of select statement
            .ToListAsync();

        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<IEnumerable<GetBookingDto>>>GetUserBookingsForHotelAsync(int hotelId)
    {
        var userId = usersService.UserId;
        var hotelsExists = await context.Hotels.AnyAsync(h => h.HotelId == hotelId);

        if(!hotelsExists)
            return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{hotelId}' was not found."));

        var bookings = await context.Bookings
            .Where(b => b.BookingId == hotelId && b.UserId == userId)
            .OrderBy(b => b.CheckInDate)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }


    public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto dto)
    {
        var userId = usersService.UserId;

        bool overlaps = await IsOverLap(dto.HotelId, userId, dto.CheckInDate, dto.CheckOutDate);

        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "The selected dates overlap with an existing booking."));
        }

        var hotel = await context.Hotels
            .Where(h => h.HotelId == dto.HotelId)
            .FirstOrDefaultAsync();

        if (hotel is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{dto.HotelId}' was not found"));
        }

        var nights = dto.CheckOutDate.DayNumber - dto.CheckInDate.DayNumber;
        var totalPrice = hotel.PerNightRate * nights;

        //When all checks have been satisfied, create booking
        var booking = mapper.Map<Booking>(dto);
        booking.UserId = userId;
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);

        return Result<GetBookingDto>.Success(result);
    }

    public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto dto)
    {
        var userId = usersService.UserId;

        bool overlaps = await IsOverLap(hotelId,userId, dto.CheckInDate, dto.CheckOutDate);

        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "The selected dates overlap with an existing booking."));
        }

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b =>
            b.BookingId == bookingId
            && b.HotelId == hotelId
            && b.UserId == userId
            );

        if (booking is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found"));
        }

        mapper.Map(dto, booking);
        var nights = dto.CheckOutDate.DayNumber - dto.CheckInDate.DayNumber;
        var perNight = booking.Hotel!.PerNightRate;
        booking.CheckInDate = dto.CheckInDate;
        booking.CheckOutDate = dto.CheckOutDate;
        booking.Guests = dto.Guests;
        booking.TotalPrice = perNight * nights;
        booking.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var updated = mapper.Map<GetBookingDto>(booking);

        return Result<GetBookingDto>.Success(updated);
    }

    private async Task<bool> IsOverLap(int hotelId, string userId, DateOnly checkIn, DateOnly checkOut, int? bookingId = null)
    {
        var query =  context.Bookings
        .Where(
            b => b.HotelId == hotelId
            && b.Status !=BookingStatus.Cancelled
            && checkIn < b.CheckOutDate
            && checkOut > b.CheckInDate
            && b.UserId == userId)
            .AsQueryable();

        if(bookingId.HasValue)
        {
            query = query.Where(q => q.BookingId !=bookingId.Value);
        }
        return await query.AnyAsync();
        
    }

    public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;

        var booking = await context.Bookings
        .Include(b => b.Hotel)
        .FirstOrDefaultAsync(b =>
        b.BookingId == bookingId
        && b.HotelId == hotelId
        && b.UserId == userId
        );

        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return Result.Failure(new Error(ErrorCodes.Conflict, "This booking has already been canncelled."));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;

        var isHotelAdmin = await context.HotelAdmins
        .AnyAsync(q => q.UserId == userId && q.HotelId == hotelId);

        if (!isHotelAdmin)
        {
            return Result.Failure(new Error(ErrorCodes.Forbid, $"You are not an admin of the selected hotel."));
        }


        var booking = await context.Bookings
        .Include(b => b.Hotel)
        .FirstOrDefaultAsync(b =>
        b.BookingId == bookingId
        && b.HotelId == hotelId
        );

        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return Result.Failure(new Error(ErrorCodes.Conflict, "This booking has already been canncelled."));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;

        var isHotelAdmin = await context.HotelAdmins
        .AnyAsync(q => q.UserId == userId && q.HotelId == hotelId);

        if (!isHotelAdmin)
        {
            return Result.Failure(new Error(ErrorCodes.Forbid, $"You are not an admin of the selected hotel."));
        }


        var booking = await context.Bookings
        .Include(b => b.Hotel)
        .FirstOrDefaultAsync(b =>
        b.BookingId == bookingId
        && b.HotelId == hotelId
        );

        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking '{bookingId}' was not found."));
        }

        if (booking.Status == BookingStatus.Confirmed)
        {
            return Result.Failure(new Error(ErrorCodes.Conflict, "This booking has already been confirmed."));
        }

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }
}