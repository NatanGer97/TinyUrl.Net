using TinyUrl.Models;

namespace TinyUrl.Services.interfaces
{
    public interface IUserClickService
    {
        Task AddNewClickAsync(UserClick click);
        Task<List<UserClick>> GetAllClicksAsync();


        Task<PagedData<UserClick>> ClicksBetweenDates(string? username, string? from, string? to, string? tinyUrl, int pageNum, int pageSize);
        Task<PagedData<UserClick>> UserClicks(string username, int pageNum, int pageSize);
        Task<PagedData<UserClick>> UserClicksByTinyUrl(string? username, string tinycode, int pageNum, int pageSize);
    }
}
