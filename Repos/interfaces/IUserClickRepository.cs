using TinyUrl.Models;

namespace TinyUrl.Repos.interfaces
{
    public interface IUserClickRepository
    {
        Task AddNewClickAsync(UserClick click);
        Task<List<UserClick>> GetAllClickesAsync();
    }
}
