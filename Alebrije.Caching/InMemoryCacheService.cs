using System;
using System.Threading.Tasks;
using Alebrije.Abstractions.Patterns;
using Microsoft.Extensions.Caching.Memory;

namespace Alebrije.Caching
{
    /// <summary>
    /// Implementation of <see cref="ICacheService"/> using a <see cref="MemoryCache"/> object.
    /// </summary>
    public class InMemoryCacheService : ICacheService
    {
        private readonly MemoryCache _cache = new(new MemoryCacheOptions());

        /// <inheritdoc cref="ICacheService"/>
        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ICacheService"/>
        public Task<T> GetAsync<T>(string key)
        {
            var itemFound = _cache.TryGetValue(key, out var stored);

            if (itemFound && stored is T result)
            {
                return Task.FromResult(result);
            }

            try
            {
                result = (T)Convert.ChangeType(stored, typeof(T));
            }
            catch
            {
                result = default;
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc cref="ICacheService"/>
        public Task SetAsync<T>(string key, T value)
        {
            _cache.Set(key, value);
            return Task.CompletedTask;
        }
    }
}