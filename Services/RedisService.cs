using MongoDB.Driver.Core.Connections;
using Newtonsoft.Json;
using StackExchange.Redis;
using StudentsDashboard.Errors;
using TinyUrl.Models;
using TinyUrl.Services.interfaces;

namespace TinyUrl.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer redisConnection;
        private readonly ILogger<RedisService> logger;
        private readonly IDatabase redis;

        public RedisService(IConnectionMultiplexer redisConnection, ILogger<RedisService> logger)
        {
            this.redisConnection = redisConnection;
            this.logger = logger;
            redis = redisConnection.GetDatabase();
        }

        /// <summary>
        /// get tinyurl obj value by tiny code from redis
        /// </summary>
        /// <param name="tinyCode"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">if key (tiny code) not exsit in redis</exception>
        public TinyUrlFromRedis? GetTinyUrlObjByCode(string tinyCode)
        {
            if (!isTinyCodeExist(tinyCode))
            {
                string message = "Key " + tinyCode + " not exist";

                logger.LogError(message);
                throw new NotFoundException(message);
            }

            RedisValue redisValue = redis.StringGet(tinyCode);
            TinyUrlFromRedis? tinyUrlFromRedis = JsonConvert.DeserializeObject<TinyUrlFromRedis>(redisValue.ToString());

            return tinyUrlFromRedis;
        }
        /// <summary>
        /// test if the tiny code exist in redis
        /// </summary>
        /// <param name="tinyCode"></param>
        /// <returns> true if exsit, false if not </returns>
        public bool isTinyCodeExist(string tinyCode)
        {
            return redis.KeyExists(tinyCode);
        }


    }
}
