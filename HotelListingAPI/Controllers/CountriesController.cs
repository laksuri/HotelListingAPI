using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListingAPI.Data;
using AutoMapper;
using HotelListingAPI.Contracts;
using HotelListingAPI.Model.Country;
using Microsoft.AspNetCore.Authorization;
using HotelListingAPI.Exception;
using HotelListingAPI.Model;

namespace HotelListingAPI.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository)
        {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
        }

        // GET: api/Countries
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            return await _countriesRepository.GetAllAsync();
        }
        [HttpGet("GetPagedCountries")]
        [Authorize]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery]QueryParameter parameter)
        {
            var pagedCountriesResult = await _countriesRepository.GetAllPagedResultsAsync<GetCountryDto>(parameter);
            return Ok(pagedCountriesResult);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
          
            var country = await _countriesRepository.GetAsync(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, GetCountryDto coutryDto)
        {
            if (id != coutryDto.Id)
            {
                return BadRequest();
            }

            try
            {
                var country= await _countriesRepository.GetAsync(coutryDto.Id);
                if(country==null)
                {
                    throw new NotFoundException(nameof(PutCountry), coutryDto.Id);
                }
                _mapper.Map(coutryDto, country);
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
        {
          var country=_mapper.Map<Country>(countryDto);
            await _countriesRepository.CreateAsync(country);
            
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country=await _countriesRepository.GetAsync(id);
            
            if (country == null)
            {
                throw new NotFoundException(nameof(DeleteCountry), id);
            }

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exists(id);
        }
    }
}
