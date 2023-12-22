using Microsoft.EntityFrameworkCore;
namespace HotelListingAPI.Data
{
    public class HotelDbContext:DbContext
    {
        public HotelDbContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "India",
                    ShortName = "IND"
                },
                new Country
                {
                    Id = 2,
                    Name = "Dubai",
                    ShortName = "DXB"
                },
                new Country
                { 
                    Id = 3,
                    Name = "Singapore",
                    ShortName = "SIN"
                }
                );
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Lemon Tree",
                    Address = "Hyderabad",
                    CountryId = 1
                },
                new Hotel
                {
                    Id=2,
                    Name="Taj Dubai",
                    Address="Dubai",
                    CountryId=2
                });
        }
    }
}
