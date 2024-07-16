namespace HotelListingAPI.Exception
{
    public class NotFoundException:ApplicationException
    {
        public NotFoundException(string name,object key):base($"{name} with {key} not found")
        {
            
        }
    }
}
