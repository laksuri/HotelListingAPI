using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Model.Country;

namespace HotelListingAPI.Config
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
        }
    }
}
