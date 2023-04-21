using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using TinyUrl.Models;
using TinyUrl.Models.Dto;

namespace TinyUrl.Services
{
    public class UserService 
    {
        
        private readonly IMongoCollection<User> usersCollection;
        private readonly IMapper mapper;

        public UserService(IOptions<MongoDBSettings> mongoDbSettings, IMapper mapper)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            /*createCollection(mongoDatabase);*/
            this.usersCollection = mongoDatabase.GetCollection<User>(mongoDbSettings.Value.UserCollectionName);

            this.mapper = mapper;
        }

        private void createCollection(IMongoDatabase mongoDatabase)
        {
            List<string> list = mongoDatabase.ListCollectionNames().ToList();

            if (!list.Exists(name => name.Equals("Users")))
            {
                mongoDatabase.CreateCollection("Users");
                Console.WriteLine("collection created");
            }
            else
            {
                Console.WriteLine("collection exists");
            }
        }

    

        public async Task<List<User>> GetUsersAync()
        {
            return await usersCollection.Find(_ => true).ToListAsync();
            
        }

        /// <summary>
        /// find user by email or null
        /// </summary>
        /// <param name="email"></param>
        /// <returns>user or null</returns>
        public async Task<User?> FindUserByUserNameAsync(string username)
        {
            return await usersCollection.Find(user => user.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUser(UserRegisterReqDto registerReqDto)
        {
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            string hashedPassword = passwordHasher.HashPassword(string.Empty, registerReqDto.Password);

            User newUser = new User() { UserName = registerReqDto.Email, Password = hashedPassword, FullName = registerReqDto.Name };

            await usersCollection.InsertOneAsync(newUser);

            return await usersCollection.Find(user => user.UserName == newUser.UserName).FirstAsync();        

        }

        public async Task<User?> FindUserByIdAsync(string userId)
        {
            return await usersCollection.Find(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<bool> IncrementMongoField(string username, string key)
        {
            FilterDefinition<User> filterDefinition = Builders<User>.Filter.Eq("UserName", username);
            UpdateDefinition<User> updateDefinition = Builders<User>.Update.Inc(key, 1);
            UpdateResult updateResult = await usersCollection.UpdateOneAsync(filterDefinition, updateDefinition);

            return updateResult.ModifiedCount == 1;
        }

        public async Task<bool> AddTinyUrlToUser(string tinyurl, string username)
        {
            FilterDefinition<User> filterDefinition = Builders<User>.Filter.Eq("UserName", username);
            UpdateDefinition<User> updateDefinition = Builders<User>.Update.AddToSet("urls", tinyurl);

            UpdateResult updateResult = await usersCollection.UpdateOneAsync(filterDefinition, updateDefinition);

            return updateResult.ModifiedCount == 1;

        }
    }
}
