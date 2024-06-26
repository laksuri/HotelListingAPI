using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Model.Country;
using HotelListingAPI.Model.User;

namespace HotelListingAPI.Config
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();

            CreateMap<APIUser, APIUserDto>().ReverseMap();
        }
    }
}
