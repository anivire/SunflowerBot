using Newtonsoft.Json;
using DSharpPlus.Entities;

namespace SunflowerBot.Commands
{
    public class UserProfile
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("profileId")]
        public ulong ProfileId { get; set; }

        [JsonProperty("sunCount")]
        public int SunCount { get; set; }
    }
}