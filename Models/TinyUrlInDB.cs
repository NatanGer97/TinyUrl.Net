using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrl.Models
{
    public class TinyUrlInDB
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Tiny { get; set; }
        public string OriginalUrl { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }


        public TinyUrlInDB()
        {
            CreatedAt = DateTime.Now.Date;
        }
        
        

    }
}
