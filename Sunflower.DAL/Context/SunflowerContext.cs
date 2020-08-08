using Microsoft.EntityFrameworkCore;
using Sunflower.DAL.Models;
using Sunflower.Sunflower.DAL.Models;

namespace Sunflower.DAL.Context
{
    class SunflowerContext : DbContext
    {
        public DbSet<Profile> UserProfiles { get; set; }
        public DbSet<SunnyMessage> SunnyMessage { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=SunflowerUsers.db");
    }
}
