using System;
using System.Collections.Generic;
using System.Text;

namespace Sunflower.DAL.Models
{
    class Profile : Entity
    {
        public ulong MemberId { get; set; }
        public string MemberUsername { get; set; }
        public int MemberSunCount { get; set; }
        public DateTime DailyCooldown { get; set; }
    }
}
