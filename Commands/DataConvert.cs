using DSharpPlus;
using Newtonsoft.Json;
using DSharpPlus.Entities;

public class UserProfile
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("profileId")]
    public DiscordMember ProfileId { get; set; }

    [JsonProperty("sunCount")]
    public int SunCount { get; set; }
}