using AutoMapper;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.Model.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<APIUser> _usermanager;
        private readonly object _configuration;
        private APIUser _user;

        public AuthManager(IMapper mapper,UserManager<APIUser> usermanager,IConfiguration configuration)
        {
            _mapper = mapper;
            _usermanager = usermanager;
            _configuration = configuration;
        }

        public async Task<IEnumerable<IdentityError>> Register(APIUserDto user)
        {
           //Map it to API User
           _user=_mapper.Map<APIUser>(user);
            _user.UserName = user.Email;

            //Create user with the username and password
            var result = await _usermanager.CreateAsync(_user, user.Password);

            //if result succeeded
            if(result.Succeeded)
            {
                result = await _usermanager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;
        }
    }
}
