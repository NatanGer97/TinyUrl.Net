
using Microsoft.EntityFrameworkCore;
using TinyUrl.Contexts;
using TinyUrl.Models;
using TinyUrl.Repos.interfaces;

namespace TinyUrl.Repos
{
    public class UserClicksRepository : BaseRepository, IUserClickRepository
    {
        public UserClicksRepository(AppDbContext context) : base(context) { }
        
        public async Task AddNewClickAsync(UserClick click)
        {
            await _context.UserClicks.AddAsync(click);
        }

        public async Task<List<UserClick>> GetAllClickes()
        {
            return await _context.UserClicks.ToListAsync();
        }
        
    }
}
