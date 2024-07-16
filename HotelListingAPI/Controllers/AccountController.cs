using AutoMapper;
using HotelListingAPI.Contracts;
using HotelListingAPI.Model.User;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public AccountController(IMapper mapper, IAuthManager authManager)
        {
            _mapper = mapper;
            _authManager = authManager;
        }
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register(APIUserDto apiUserDto)
        {
            var errors = await _authManager.Register(apiUserDto);
            if(errors.Any())
            {
                foreach(var error in errors)
                {
                    ModelState.AddModelError(error.Code,error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok();
        }
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var authResponse= await _authManager.Login(loginDto);

            if(authResponse==null)
            {
                return Unauthorized();
            }
            return Ok(authResponse);
        }
    }
}
