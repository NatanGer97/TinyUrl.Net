
using Microsoft.EntityFrameworkCore;
using TinyUrl.Models;
using TinyUrl.Contexts;
using TinyUrl.Models;
using TinyUrl.Repos.interfaces;
using ZstdSharp.Unsafe;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace TinyUrl.Repos
{
    public class UserClicksRepository : BaseRepository, IUserClickRepository
    {
        public UserClicksRepository(AppDbContext context) : base(context) { }

        public async Task AddNewClickAsync(UserClick click)
        {
            await _context.UserClicks.AddAsync(click);
        }

        public async Task<IEnumerable<UserClick>> AllClicks()
        {
            return await _context.UserClicks.ToListAsync();
        }

        public async Task<PagedData<UserClick>> ClicksBetweenDates(string? username, string? from, string? to, string? tinyUrl, int pageNum, int pageSize)
        {
            var content = await _context.UserClicks
                .Where(click => click.Username.Equals(username))
                .Where(click => click.ClickedAt >= DateTime.Parse(from).ToUniversalTime())
                .Where(click => click.ClickedAt <= DateTime.Parse(to).ToUniversalTime())
                .Where(click => click.TinyUrl.Equals(tinyUrl))
                .Skip((pageNum - 1) * pageSize).Take(pageSize)
                .ToListAsync();
            var contentSize = _context.UserClicks
                .Where(click => click.Username.Equals(username))
                .Where(click => click.ClickedAt >= DateTime.Parse(from).ToUniversalTime())
                .Where(click => click.ClickedAt <= DateTime.Parse(to).ToUniversalTime())
                .Where(click => click.TinyUrl.Equals(tinyUrl)).Count();

            int totalPages = (int)Math.Ceiling((double)contentSize / pageSize);
            if (totalPages < 1) totalPages = 1;

            return PagedData<UserClick>.ToPagedData(content, pageNum, totalPages);
        }

        public async Task<List<UserClick>> GetAllClickesAsync()
        {
            return await _context.UserClicks.ToListAsync();
        }

        public async Task<PagedData<UserClick>> UserClicks(string username, int pageNum, int pageSize)
        {

            var content = await _context.UserClicks.Where(click => click.Username.Equals(username))
                .Skip((pageNum - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling((double)(_context.UserClicks.Count()) / pageSize);
            if (totalPages < 1) totalPages = 1;

            return PagedData<UserClick>.ToPagedData(content, pageNum, totalPages);
        }

        public async Task<PagedData<UserClick>> UserClicksByTinyUrl(string? username, string tinycode, int pageNum, int pageSize)
        {
            var content = await _context.UserClicks.Where(click => click.Username.Equals(username) && click.TinyUrl.Equals(tinycode))
            .Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            var contentSize = _context.UserClicks.Where(click => click.Username.Equals(username) && click.TinyUrl.Equals(tinycode)).Count();
            int totalPages = (int)Math.Ceiling((double)contentSize / pageSize);
            if (totalPages < 1) totalPages = 1;

            return PagedData<UserClick>.ToPagedData(content, pageNum, totalPages);
        }
    }
}
