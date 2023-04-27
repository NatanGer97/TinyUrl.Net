using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrl.Models.Dto
{
    public class UserDtoOut
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public Dictionary<string, object>? UserStats { get; set; }






        public UserDtoOut()
        {
            
        }

    }
}
