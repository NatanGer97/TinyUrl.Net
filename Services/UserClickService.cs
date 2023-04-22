using TinyUrl.UnitOfWork;
using TinyUrl.Models;
using TinyUrl.Repos.interfaces;
using TinyUrl.Services.interfaces;

namespace TinyUrl.Services
{
    public class UserClickService : IUserClickService
    {
        private readonly IUserClickRepository userClickRepository;
        private readonly IUnitOfWork unitOfWork;
        

        public UserClickService(IUserClickRepository userClickRepository, IUnitOfWork unitOfWork)
        {
            this.userClickRepository = userClickRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task AddNewClickAsync(UserClick click)
        {
            await userClickRepository.AddNewClickAsync(click);
            await unitOfWork.CompleteAsync();
        }

        public async Task<PagedData<UserClick>> ClicksBetweenDates(string? username, string? from, string? to, string? tinyUrl, int pageNum, int pageSize)
        {
            return await userClickRepository.ClicksBetweenDates(username, from, to, tinyUrl, pageNum, pageSize);
        }

    
        public async Task<List<UserClick>> GetAllClicksAsync()
        {
             List<UserClick> userClicks = await userClickRepository.GetAllClickesAsync();

            return userClicks;
        }

        public async Task<PagedData<UserClick>> UserClicks(string username, int pageNum, int pageSize)
        {
            return await userClickRepository.UserClicks(username, pageNum, pageSize);
        }

        public async Task<PagedData<UserClick>> UserClicksByTinyUrl(string? username, string tinycode, int pageNum, int pageSize)
        {
            return await userClickRepository.UserClicksByTinyUrl(username, tinycode, pageNum, pageSize);
        }
    }
}
