using Newtonsoft.Json;
using System.IO;
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

        public void Save(string path)
        {
            File.AppendAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static UserProfile Load(string path)
        {
            return JsonConvert.DeserializeObject<UserProfile>(File.ReadAllText(path));
        }
    }
}