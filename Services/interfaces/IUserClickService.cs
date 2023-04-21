using TinyUrl.Models;

namespace TinyUrl.Services.interfaces
{
    public interface IUserClickService
    {
        Task AddNewClickAsync(UserClick click);
        Task<List<UserClick>> GetUserClicksAsync();
    }
}
