using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingAPI.Config;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace HotelListingAPI.Repository
{
    public class GenericRepository<T>:IGenericRepository<T> where T:class
    {
        private readonly HotelDbContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(HotelDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
             _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        async Task<PagedResult<TResult>> IGenericRepository<T>.GetAllPagedResultsAsync<TResult>(QueryParameter queryParameter)
        {
            int totalsize=await _context.Set<T>().CountAsync();
            var items = await _context.Set<T>()
                .Skip(queryParameter.StartIndex)
                .Take(queryParameter.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TResult>
            {
                Items = items,
                TotalCount = totalsize
            };

        }
    }
}
