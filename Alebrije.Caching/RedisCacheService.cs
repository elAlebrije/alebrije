using System.Threading.Tasks;
using Alebrije.Abstractions.Patterns;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Alebrije.Caching
{
    /// <summary>
    /// Implementation of <see cref="ICacheService"/> using a Redis Server.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public T Get<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool Set<T>(string key, T value)
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var json = await db.StringGetAsync(key);
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var json = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, json);
        }
    }
}