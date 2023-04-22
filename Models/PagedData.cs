namespace TinyUrl.Models
{
    public class PagedData<T>
    {
        public int Page { get; set; }
        public int Pages { get; set; }

        public IEnumerable<T>? Data { get; set; }

        public PagedData(IEnumerable<T> data, int page, int pages)
        {
            Data = data;
            Page = page;
            Pages = pages;
        }
        public PagedData()
        {

        }

        public static PagedData<T> ToPagedData(IEnumerable<T> data, int page, int pages)
        {
            return new PagedData<T>(data, page, pages);
        }
        
    }
}
