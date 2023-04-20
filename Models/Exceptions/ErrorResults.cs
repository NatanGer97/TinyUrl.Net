namespace StudentsDashboard.Errors
{
    public class ErrorResults
    {
        public List<string>? Messages { get; set; }
        public string? Source { get; set; }
        public string? Exception { get; set; }
        public string? ErrorId { get; set; }
        public string? SupportMessage { get; set; }
        public int StatusCode { get; set; }

        public ErrorResults()
        {
            Messages = new List<string>();
        }
    }
}
