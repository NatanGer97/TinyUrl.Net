using StudentsDashboard.UnitOfWork;
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

    
        public async Task<List<UserClick>> GetUserClicksAsync()
        {
             List<UserClick> userClicks = await userClickRepository.GetAllClickesAsync();

            return userClicks;
        }
    }
}
