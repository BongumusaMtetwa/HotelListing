using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.AuthorizationFilter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/hotel/{hotelId:int}/bookings")]
[ApiController]
[Authorize]
public class HotelBookingsController(IBookingService bookingService) : BaseApiController
{
    // "api/hotels/5/bookings"
    [HttpGet("admin")]
    [HotelOrSystemAdmin]
    public async Task<ActionResult<IEnumerable<GetBookingDto>>> GetBookings([FromRoute] int hotelId)
    {
        //Do the srvice call and return data

       var result = await bookingService.GetBookingsForHotelAsync(hotelId);
       return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<GetBookingDto>> CreateBooking([FromRoute] int hotelId, [FromBody] CreateBookingDto createBookingDto)
    {
        //Do the service call and return data
        var result = await bookingService.CreateBookingAsync(createBookingDto);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}")]
    public async Task<ActionResult<GetBookingDto>>UpdateBooking([FromRoute]int hotelId, [FromRoute] int bookingId,[FromBody] UpdateBookingDto updateBookingDto)
    {
        var result = await bookingService.UpdateBookingAsync(hotelId, bookingId, updateBookingDto);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}/cancel")]
    public async Task<IActionResult>CancelBooking([FromRoute] int hotelId, [FromRoute] int bookingId)
    {
        var result = await bookingService.CancelBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}/admin/cancel")]
    [HotelOrSystemAdmin]
    public async Task<IActionResult>CancelBookingAsync([FromRoute] int hotelId, [FromRoute] int bookingId)
    {
        var result = await bookingService.CancelBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }

     [HttpPut("{bookingId:int}/admin/confirm")]
     [HotelOrSystemAdmin]
    public async Task<IActionResult>ConfirmBookingAsync([FromRoute] int hotelId, [FromRoute] int bookingId)
    {
        var result = await bookingService.CancelBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }
}
