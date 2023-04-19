using Amazon.Runtime.Internal.Util;
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

        public UserService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            createCollection(mongoDatabase);
            this.usersCollection = mongoDatabase.GetCollection<User>("Users");
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

    

        public async Task<List<User>> GetUserAync()
        {
            return await usersCollection.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// find user by email or null
        /// </summary>
        /// <param name="email"></param>
        /// <returns>user or null</returns>
        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await usersCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUser(UserRegisterReqDto registerReqDto)
        {
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            string hashedPassword = passwordHasher.HashPassword(string.Empty, registerReqDto.Password);

            User newUser = new User() { Email = registerReqDto.Email, Password = hashedPassword, Name = registerReqDto.Name };

            await usersCollection.InsertOneAsync(newUser);

            return await usersCollection.Find(user => user.Email == newUser.Email).FirstAsync();

          


        }
    }
}
