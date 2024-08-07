﻿using HotelListingAPI.Model;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;

namespace HotelListingAPI.Contracts
{
    public interface IGenericRepository<T> where T:class
    {
        Task<T> GetAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<PagedResult<TResult>> GetAllPagedResultsAsync<TResult>(QueryParameter queryParameter);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> Exists(int id);
    }
}
