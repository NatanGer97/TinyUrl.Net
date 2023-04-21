namespace TinyUrl.Models
{
    public class UserClick
    {
        
        public long Id { get; set; }
        public string  Username { get; set; }
        public string TinyUrl { get; set; }
        public string OriginalUrl { get; set; }
        public string ClickedAt { get; set; }
    }
}
