using DSharpPlus.Entities;
using Sunflower.DAL.Models;

namespace Sunflower.Sunflower.DAL.Models
{
    class SunnyMessage : Entity
    {
        public ulong GuildId { get; set; }
        public ulong MessageId { get; set; }
        public ulong RoleId { get; set; }
    }
}
