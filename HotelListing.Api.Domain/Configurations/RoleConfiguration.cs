using HotelListing.Api.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Data.Cofigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "3cbe1352-4b9d-4548-9047-3937c2918252",
                    Name = RoleNames.Administrator,
                    NormalizedName = RoleNames.Administrator.ToUpper()
                },
                new IdentityRole
                {
                    Id = "452614c8-9305-48fd-9de1-3eaec98dd7c4",
                    Name = RoleNames.User,
                    NormalizedName = RoleNames.User.ToUpper()
                },
                new IdentityRole
                {
                    Id = "c52da705-9c7c-45b6-921f-1254c42b85f8",
                    Name = RoleNames.HotelAdmin,
                    NormalizedName = RoleNames.HotelAdmin.ToUpper()
                }

            );
        }
    }

}