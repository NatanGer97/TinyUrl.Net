using TinyUrl.Models;

namespace TinyUrl.Repos.interfaces
{
    public interface IUserClickRepository
    {
        Task AddNewClickAsync(UserClick click);
        Task<List<UserClick>> GetAllClickesAsync();

        Task<IEnumerable<UserClick>> AllClicks();

        Task<PagedData<UserClick>> ClicksBetweenDates(string? username, string? from, string? to, string? tinyUrl, int pageNum, int pageSize);
        Task<PagedData<UserClick>> UserClicks(string username, int pageNum, int pageSize);
        Task<PagedData<UserClick>> UserClicksByTinyUrl(string? username,string tinycode, int pageNum, int pageSize);
        



    }
}
