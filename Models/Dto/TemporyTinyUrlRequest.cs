using Newtonsoft.Json;
using TinyUrl.Models.Enums;

namespace TinyUrl.Models.Dto
{
    public class TemporyTinyUrlRequest
    {
        public string Username { get; set; }
        public string OriginalUrl { get; set; }

        [JsonIgnore]
        public eTinyUrlTimeToLiveKey timeToLiveKey{ get; set; }

        [JsonIgnore]
        public int TimeToLive { get; set; } 




    }
}
