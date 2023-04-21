using TinyUrl.Contexts;

namespace TinyUrl.Repos
{
    public abstract class BaseRepository 
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

    }
}
