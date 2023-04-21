using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;
using StudentsDashboard.Errors;
using System.Text;
using System.Text.Json;
using TinyUrl.Builders;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Models.Responses;
using TinyUrl.Services.interfaces;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace TinyUrl.Services
{
    public class UrlService : IUrlService
    {
        private const int TINY_SIZE = 4;
        private const int NUM_OF_RETRIES = 5;
        private readonly string CHARS = "ABCDEFHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        private readonly IConnectionMultiplexer redisConnection;        
        private readonly IDatabase redis;
        private readonly IMongoCollection<TinyUrlInDB> tinyUrlCollection;

        public UrlService(IConnectionMultiplexer redis, IOptions<MongoDBSettings> mongoDbSettings)
        {
            this.redisConnection = redis;
            this.redis = redis.GetDatabase();

            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            this.tinyUrlCollection = mongoDatabase.GetCollection<TinyUrlInDB>(mongoDbSettings.Value.TinyUrlCollectionName);
        }

        public async Task<string> CreateNewTinyUrlAsync(NewTinyUrlReq newTinyUrlReq)
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
            /*bool success = redis.StringSet(tinyCode, JsonConvert.SerializeObject(tinyUrlReq));*/

            TinyUrlInDB tinyUrlInDB = await addToDatabasesAsync(tinyCode, newTinyUrlReq);
            
            if (tinyUrlInDB == null)
            {
                throw new InternalServerException("error while setting key into redis");
            }

            return "https://localhost:7112/" + tinyCode; ;
        }

        private async Task<TinyUrlInDB> addToDatabasesAsync(string tinycode, NewTinyUrlReq newTinyUrlReq)
        {
            TinyUrlInDB tinyUrl = new TinyUrlInDB
            {
                CreatedAt = DateTime.Now,
                OriginalUrl = newTinyUrlReq.Url,
                Tiny = tinycode,
                Username = newTinyUrlReq.Username
            };
            
            redis.StringSet(tinycode, JsonConvert.SerializeObject(newTinyUrlReq));
            TinyUrlInDB tinyUrlInDB = await addToTinyUrlsCollectionAsync(tinycode, tinyUrl);

            return tinyUrlInDB;

        }

        private async Task<TinyUrlInDB> addToTinyUrlsCollectionAsync(string tinycode, TinyUrlInDB tinyUrl)
        {
            await tinyUrlCollection.InsertOneAsync(tinyUrl);
            TinyUrlInDB tinyUrlInDB = await tinyUrlCollection.Find(url => url.Tiny == tinycode).FirstAsync();

            return tinyUrlInDB;
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
