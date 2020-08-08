using Microsoft.EntityFrameworkCore;
using Sunflower.DAL.Models;

namespace Sunflower.DAL.Context
{
    class DatabaseContext : DbContext
    {
        public DbSet<Profile> UserProfiles { get; set; }
        public DbSet<MessageReaction> SunnyMessage { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=SunflowerUsers.db");
    }
}
