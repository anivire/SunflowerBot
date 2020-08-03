using Microsoft.EntityFrameworkCore;
using Sunflower.Models;

namespace Sunflower.Context
{
    class SunflowerUsersContext : DbContext
    {
        public DbSet<Profile> UserProfiles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=SunflowerUsers.db");
    }
}
