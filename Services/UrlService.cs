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
using TinyUrl.Models.Enums;
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
       
        private readonly IMongoCollection<TinyUrlInDB> tinyUrlCollection;
        private readonly IRedisService redisService;
        private readonly UserService userService;

        public UrlService( IRedisService redisService, UserService userService,
            IOptions<MongoDBSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            this.tinyUrlCollection = mongoDatabase.GetCollection<TinyUrlInDB>(mongoDbSettings.Value.TinyUrlCollectionName);
            this.redisService = redisService;
            this.userService = userService;
        }

        public async Task<string> CreateNewTinyUrlAsync(NewTinyUrlReq newTinyUrlReq)
        {

            string tinyCode = generateTinyCode();
            int counter = 0;

            while (redisService.isTinyCodeExist(tinyCode) && counter++ < NUM_OF_RETRIES)
            {
                tinyCode = generateTinyCode();
            }

            if (counter == NUM_OF_RETRIES)
            {
                throw new InternalServerException("Can't generate tiny url, all options are taken");
            }

           
            TinyUrlInDB tinyUrlInDB = await addToDatabasesAsync(tinyCode, newTinyUrlReq);
            
            if (tinyUrlInDB == null)
            {
                throw new InternalServerException("error while setting key into redis");
            }

            return "https://localhost:7112/" + tinyCode; ;
        }

        public async Task OnUrlClickAsync(string tinyUrl, string username)
        {
            await userService.IncrementClickField(username, string.Empty, eKeys.UserClicks);
            await userService.IncrementClickField(username, tinyUrl, eKeys.UserTinyUrlsClicksMonth);
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
            // set to redis
            bool isSuccess = redisService.SetValue(tinycode, newTinyUrlReq);
            // add into mongo
            TinyUrlInDB tinyUrlInDB = await addToTinyUrlsCollectionAsync(tinycode, tinyUrl);

            return tinyUrlInDB;

        }

        private async Task<TinyUrlInDB> addToTinyUrlsCollectionAsync(string tinycode, TinyUrlInDB tinyUrl)
        {
            await tinyUrlCollection.InsertOneAsync(tinyUrl);
            TinyUrlInDB tinyUrlInDB = await tinyUrlCollection.Find(url => url.Tiny == tinycode).FirstAsync();

            return tinyUrlInDB;
        }


        // TODO: move out -> strategy pattern(?)
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
