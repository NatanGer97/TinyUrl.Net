using System.Net;

namespace TinyUrl.Models.Responses
{
    public class NewTinyUrlRespons : BaseResponse
    {
        public TinyUrlFromRedis tinyUrl { get; set; }

        private  NewTinyUrlRespons(bool success, string msg, TinyUrlFromRedis tinyUrl)
            : base(msg, success)
        {
            this.tinyUrl = tinyUrl; 
        }

        // success case
        public NewTinyUrlRespons(TinyUrlFromRedis tinyUrl): this(true, string.Empty, tinyUrl)
        {

        }

        // failiure case
        public NewTinyUrlRespons(string msg )
            : this(false, msg, null)
        {

        }
    }
}
