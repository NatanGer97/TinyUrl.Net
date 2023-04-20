using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using StackExchange.Redis;
using StudentsDashboard.Errors;
using System.Text;
using System.Text.Json;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Models.Responses;
using TinyUrl.Services.interfaces;

namespace TinyUrl.Services
{
    public class UrlService : IUrlService
    {
        private const int TINY_SIZE = 4;
        private const int NUM_OF_RETRIES = 5;
        private readonly string CHARS = "ABCDEFHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly IConnectionMultiplexer redisConnection;
        
        private IDatabase redis;





        public UrlService(IConnectionMultiplexer redis)
        {
            this.redisConnection = redis;
            this.redis = redis.GetDatabase();
        }

        public string CreateNewTinyUrl(NewTinyUrlReq tinyUrlReq)
        {

            string tinyCode = generateTinyCode();
            int counter = 0;

            while (isTinyCodeExist(tinyCode) && counter++ < NUM_OF_RETRIES)
            {
                tinyCode = generateTinyCode();
            }

            if (counter == NUM_OF_RETRIES)
            {
                throw new InternalServerException("Can't generate tiny url, all options are taken");
            }

            // set into redis
            bool success = redis.StringSet(tinyCode, JsonConvert.SerializeObject(tinyUrlReq));

            if (!success)
            {
                throw new InternalServerException("error while setting key into redis");
            }

            return "https://localhost:7112/" + tinyCode; ;
        }


        public TinyUrlFromRedis? GetTinyUrlObjByCode(string tinyCode)
        {
            if (!isTinyCodeExist(tinyCode))
            {
                throw new NotFoundException("Key " + tinyCode + " not exist");
            }

            RedisValue redisValue = redis.StringGet(tinyCode);
            TinyUrlFromRedis? tinyUrlFromRedis = JsonConvert.DeserializeObject<TinyUrlFromRedis>(redisValue.ToString());

            return tinyUrlFromRedis;
        }
        

        public bool isTinyCodeExist(string tinyCode)
        {
            return redis.KeyExists(tinyCode);
        }
        

        private string generateTinyCode()
        {
            StringBuilder code = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < TINY_SIZE; i++)
            {
                code.Append(CHARS.ElementAt(random.Next(CHARS.Length)));
            }

            return code.ToString();
        }
    }
}
