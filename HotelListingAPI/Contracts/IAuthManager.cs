using HotelListingAPI.Model.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(APIUserDto user);
        
        Task<AuthResponseDto> Login(LoginDto loginDto);

    }
}
