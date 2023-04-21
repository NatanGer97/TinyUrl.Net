using TinyUrl.Models;

namespace TinyUrl.Builders
{
    public class TinyUrlDbBuilder
    {
        private string Id { get; set; }
        private string Tiny { get; set; }
        private string OriginalUrl { get; set; }
        private DateTime CreatedAt { get; set; }
        private string Username { get; set; }

        private TinyUrlDbBuilder() { }

        public static TinyUrlDbBuilder Builder()
        {
            return new TinyUrlDbBuilder();
        }

        public TinyUrlDbBuilder WithId(string id)
        {
            Id = id;
            return this;
        }

        public TinyUrlDbBuilder WithTiny(string tiny)
        {
            Tiny = tiny;
            return this;
        }

        public TinyUrlDbBuilder WithOriginalUrl(string originalUrl)
        {
            OriginalUrl = originalUrl;
            return this;
        }

        public TinyUrlDbBuilder WithCreatedAt(DateTime createdAt)
        {
            CreatedAt = createdAt;
            return this;
        }

        public TinyUrlDbBuilder WithUsername(string username)
        {
            Username = username;
            return this;
        }

        public TinyUrlInDB Build()
        {
            TinyUrlInDB tinyUrl = new TinyUrlInDB
            {
                CreatedAt = this.CreatedAt,
                Id = this.Id,
                OriginalUrl = this.OriginalUrl,
                Tiny = this.Tiny,
                Username = this.Username
            };

            return tinyUrl;
        }


    }
}
