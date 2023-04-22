namespace TinyUrl.Models
{
    public class UserClick
    {

        public long Id { get; set; }
        public string? Username { get; set; }
        public string? TinyUrl { get; set; }
        public string? OriginalUrl { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

        public static UserClick UserClickFrom(string username, string tinyUrl, string originalUrl, DateTime clickedAt)
        {
            UserClick userClick = new UserClick()
            {
                Username = username,
                TinyUrl = tinyUrl,
                OriginalUrl = originalUrl,
                ClickedAt = clickedAt
            };

            return userClick;
        }

        public static UserClick UserClickFrom(string username, string tinyUrl, string originalUrl)
        {
            return UserClickFrom(username, tinyUrl, originalUrl, DateTime.UtcNow);
        }

    }
}
