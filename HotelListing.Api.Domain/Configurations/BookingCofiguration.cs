using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Data.Cofigurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Booking> builder
    )
    {
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.HotelId);
        builder.HasIndex(x => new { x.CheckInDate, x.CheckOutDate }); //composite index for check in and check out date
    }
}
