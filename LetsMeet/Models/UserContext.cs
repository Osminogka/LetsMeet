using Microsoft.EntityFrameworkCore;

namespace LetsMeet.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> opts)
            : base(opts) { }

        public DbSet<User> Users => Set<User>();
    }
}