using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.Model.User
{
    public class APIUserDto:LoginDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
    }
}
