using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrl.Models.Dto
{
    public class UserDtoOut
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }

        public int UserClicks { get; set; }

        [BsonExtraElements]
        public Dictionary<string, int> TinyUrlsStatistic { get; set; }

        public UserDtoOut()
        {
            TinyUrlsStatistic = new Dictionary<string, int>();
            UserClicks = 0;
        }

    }
}
