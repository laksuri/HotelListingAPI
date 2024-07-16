using AutoMapper;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListingAPI.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<APIUser> _usermanager;
        private readonly IConfiguration _configuration;
        private APIUser _user;

        public AuthManager(IMapper mapper,UserManager<APIUser> usermanager,IConfiguration configuration)
        {
            _mapper = mapper;
            _usermanager = usermanager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            _user=await _usermanager.FindByEmailAsync(loginDto.Email);
            bool isValidUser=await _usermanager.CheckPasswordAsync(_user,loginDto.Password);
            if(_user==null || isValidUser==false)
            {
                return null;
            }
            var token = await GenerateToken();
            return new AuthResponseDto()
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken=await CreateRefreshToken()
            };
        }

        private async Task<string> GenerateToken()
        {
            //Generate Signing Credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]));
            var credentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            //Generate Claims
            var roles = await _usermanager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
            var userClaims = await _usermanager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id),
            }
            .Union(userClaims).Union(roleClaims);

            //Generate token
            var token =new JwtSecurityToken(
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JWTSettings:DurationInMinutes"])),
                signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

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

        public async Task<string> CreateRefreshToken()
        {
            await _usermanager.RemoveAuthenticationTokenAsync(_user, "HotelListingAPI", "RefreshToken");
            var newRefreshToken = await _usermanager.GenerateUserTokenAsync(_user, "HotelListingAPI", "RefreshToken");
            var result = await _usermanager.SetAuthenticationTokenAsync(_user, "HotelListingAPI", "RefreshToken", newRefreshToken);
            return newRefreshToken;
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler=new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var userName = tokenContent.Claims.ToList().FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _usermanager.FindByEmailAsync(userName);

            if(_user==null ||_user.Id!=request.UserId)
            {
                return null;
            }
            var isValidRefreshToken = await _usermanager.VerifyUserTokenAsync(_user, "HotelListingAPI", "RefreshToken", request.RefreshToken);
            if(isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    RefreshToken = await CreateRefreshToken(),
                    UserId = _user.Id
                };
            }
            await _usermanager.UpdateSecurityStampAsync(_user);
            return null;
        }
    }
}
