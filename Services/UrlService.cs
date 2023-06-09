﻿using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;
using TinyUrl.Errors;
using System.Text;
using System.Text.Json;
using TinyUrl.Builders;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Models.Enums;
using TinyUrl.Models.Responses;
using TinyUrl.Services.interfaces;
using TinyUrl.Strategy;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using TinyUrl.Models.Exceptions;

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
        private readonly IUserClickService userClickService;

        public UrlService(IRedisService redisService, UserService userService,
            IOptions<MongoDBSettings> mongoDbSettings, IUserClickService userClickService)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            this.tinyUrlCollection = mongoDatabase.GetCollection<TinyUrlInDB>(mongoDbSettings.Value.TinyUrlCollectionName);
            this.redisService = redisService;
            this.userService = userService;
            this.userClickService = userClickService;
        }

        public async Task<string> CreateNewTinyUrlAsync(NewTinyUrlReq newTinyUrlReq)
        {
            string tinyCode = CreateMapping();

            TinyUrlInDB tinyUrlInDB = await addToDatabasesAsync(tinyCode, newTinyUrlReq);

            if (tinyUrlInDB == null)
            {
                throw new InternalServerException("error while setting key into redis");
            }

            return "https://localhost:7112/" + tinyCode; ;
        }

        private string CreateMapping()
        {
            TinyCodeGenerator tinyCodeGenerator = new TinyCodeGenerator();
            string tinyCode = tinyCodeGenerator.GenerateCode();
            int counter = 0;

            while (redisService.isTinyCodeExist(tinyCode) && counter++ < NUM_OF_RETRIES)
            {
                tinyCode = tinyCodeGenerator.GenerateCode();
            }

            if (counter == NUM_OF_RETRIES)
            {
                throw new InternalServerException("Can't generate tiny url, all options are taken");
            }

            return tinyCode;
        }

        public async Task OnUrlClickAsync(UserClick userClick)
        {
            string tinyUrl = userClick.TinyUrl;
            string username = userClick.Username;
            // increment user clicks
            await userService.IncrementClickField(username, string.Empty, eKeys.UserClicks);
            await userService.IncrementClickField(username, tinyUrl, eKeys.UserTinyUrlsClicksMonth);
            // save new click instacne
            await userClickService.AddNewClickAsync(userClick);
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



        public async Task<List<TinyUrlInDB>> GetAllTinyCodesByUsernameAsync(string username)
        {
            // TODO: test if user exsit

            var filter = Builders<TinyUrlInDB>.Filter.Eq("Username", username);
            List<TinyUrlInDB> tinyUrlInDBs = await tinyUrlCollection
                .Find(tinyurlDocument => tinyurlDocument.Username.Equals(username))
               .ToListAsync();

            return tinyUrlInDBs;


        }


        public async Task<string> CreateTemoraryLink(TemporyTinyUrlRequest temporyTinyUrlRequest)
        {
            string username = temporyTinyUrlRequest.Username;

            User? user = await userService.FindUserByUserNameAsync(username);
            if (user == null)
            {
                throw new NotFoundException($"user with username: {username} notfound");

            }

            string tinyCode = this.CreateMapping();

            TimeSpan ttl_TimeSpan = buildTimeSpan(temporyTinyUrlRequest.timeToLiveKey, temporyTinyUrlRequest.TimeToLive);
            NewTinyUrlReq newTinyUrlReq = new NewTinyUrlReq() { Url = temporyTinyUrlRequest.OriginalUrl, Username = username };

            bool isSuccess = redisService.SetValueWithTTL(tinyCode, newTinyUrlReq, ttl_TimeSpan);
            if (!isSuccess)
            {
                throw new CustomeException("error while create temporary tinyUrl");
            }

            return "https://localhost:7112/" + tinyCode; ;

        }

        private TimeSpan buildTimeSpan(eTinyUrlTimeToLiveKey timeToLiveKey, int timeToLive)
        {
            TimeSpan timeSpan = TimeSpan.Zero;

            switch (timeToLiveKey)
            {
                case eTinyUrlTimeToLiveKey.Day:
                    timeSpan = TimeSpan.FromDays(timeToLive);
                    break;
                case eTinyUrlTimeToLiveKey.Hour:
                    timeSpan = TimeSpan.FromHours(timeToLive);
                    break;
                case eTinyUrlTimeToLiveKey.Minutes:
                    timeSpan = TimeSpan.FromMinutes(timeToLive);
                    break;
                default:
                    break;
            }

            return timeSpan;
        }






    }
}
