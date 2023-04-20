using MongoDB.Driver.Core.Connections;
using StackExchange.Redis;

namespace TinyUrl.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer redis;
        public RedisService(IConnectionMultiplexer redis)
        {
            this.redis = redis;
        }

        public IDatabase Redis()
        {
            return redis.GetDatabase();
        }
    }
}
