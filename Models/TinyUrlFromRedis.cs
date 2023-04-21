namespace TinyUrl.Models
{
    public class TinyUrlFromRedis
    {
        public string Url { get; set; }
        public string Username{ get; set; }

        public TinyUrlFromRedis()
        {

        }

        public TinyUrlFromRedis(string originalUrl, string username)
        {
            Url = originalUrl;
            Username = username;
        }
    }
}
