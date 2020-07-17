using SunflowerBot.Attributes;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Commands​Next.Converters;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace SunflowerBot.Commands
{
    public class DataCommands : BaseCommandModule
    {
        [Command("createdata")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Createdata(CommandContext ctx)
        {
            var getusersEmbed = new DiscordEmbedBuilder
            {
                Description = "Все новые пользователи были принудительно добавлены в `UsersData.json`.",
                Color = DiscordColor.Gold,
            };

            var users = ctx.Guild.Members;

            foreach (var item in users)
            {
                var profile = new UserProfile
                {
                    Username = item.Value.Username,
                    ProfileId = item.Key,
                    SunCount = 0,
                };
                File.AppendAllText(Environment.CurrentDirectory + "\\Data\\" + "\\UsersData.json", JsonConvert.SerializeObject(profile));
            }
            

            var profileCheck = JsonConvert.DeserializeObject<UserProfile>(File.ReadAllText(Environment.CurrentDirectory + "\\Data\\" + "\\UsersData.json"));
            System.Console.WriteLine(profileCheck);

            await ctx.Channel.SendMessageAsync(embed: getusersEmbed).ConfigureAwait(false);
        }
    }
}