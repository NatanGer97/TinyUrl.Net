using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using TinyUrl.Errors;
using TinyUrl.Models;
using TinyUrl.Models.Dto;
using TinyUrl.Models.Enums;

namespace TinyUrl.Services
{
    public class UserService
    {

        private readonly IMongoCollection<User> usersCollection;
        private readonly IMongoDatabase mongoDatabase;

        public UserService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            /*createCollection(mongoDatabase);*/
            this.usersCollection = mongoDatabase.GetCollection<User>(mongoDbSettings.Value.UserCollectionName);


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

        /// <summary>
        /// get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">if user not found</exception>
        public async Task<User> GetUser(string username)
        {
            User? userFromDB = await this.FindUserByUserNameAsync(username);
            if (userFromDB == null)
            {
                throw new NotFoundException($"user with username: {username} not found");                    
            }

            Console.WriteLine(userFromDB.ToJson());

            return userFromDB;
            
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

        /// <summary>
        /// increment query for clicks increamns
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tinycode">string or null if, tiny code is not part of the key</param>
        /// <param name="key"></param>
        /// <returns> <see langword="true"/> if succeess otherwise <see langword="false"/> </returns>
        public async Task<bool> IncrementClickField(string username, string tinycode, eKeys key)
        {
            FilterDefinition<User> filterDefinition = Builders<User>.Filter.Eq("UserName", username);
            UpdateDefinition<User> updateDefinition = Builders<User>.Update.Inc(createKey(key,tinycode), 1);
            UpdateResult updateResult = await usersCollection.UpdateOneAsync(filterDefinition, updateDefinition);

            return updateResult.ModifiedCount == 1;
        }
        /// <summary>
        /// increment query for clicks increamns with givven date
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tinycode"></param>
        /// <param name="key"></param>
        /// <param name="clickedAt"></param>
        /// <returns></returns>

        public async Task<bool> IncrementClickFieldWithGivenDate(string username, string tinycode, DateTime clickedAt)
        {
            FilterDefinition<User> filterDefinition = Builders<User>.Filter.Eq("UserName", username);
            UpdateDefinition<User> updateDefinition = Builders<User>.Update.Inc(tinycode + "_clicks_" + clickedAt.ToString("MM/yyyy"), 1);
            UpdateResult updateResult = await usersCollection.UpdateOneAsync(filterDefinition, updateDefinition);

            return updateResult.ModifiedCount == 1;
        }



        /// <summary>
        /// creates key to mongo increment click query
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tinycode"></param>
        /// <returns>key to the query</returns>
        private string createKey(eKeys key, string tinycode)
        {
            string results = string.Empty;
            switch (key)
            {
                case eKeys.UserClicks:
                    results = "UserClicks";
                    break;
                case eKeys.UserTinyUrlsClicksMonth:
                    results = tinycode + ".clicks." + DateTime.UtcNow.ToString("MM/yyyy");
                    break;
                default:
                    break;
            }

            return results;
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
