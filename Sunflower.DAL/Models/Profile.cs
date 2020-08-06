using System;

namespace Sunflower.DAL.Models
{
    class Profile : Entity
    {
        public ulong GuildId { get; set; }
        public ulong MemberId { get; set; }
        public string MemberUsername { get; set; }
        public int MemberSunCount { get; set; }
        public DateTime DailyCooldown { get; set; }
    }
}
