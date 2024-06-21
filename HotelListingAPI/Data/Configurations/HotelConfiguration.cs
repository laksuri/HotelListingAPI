using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Lemon Tree",
                    Address = "Hyderabad",
                    CountryId = 1
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Taj Dubai",
                    Address = "Dubai",
                    CountryId = 2
                });
        }
    }
}
