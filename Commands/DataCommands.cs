using SunflowerBot.Attributes;
using LightJson;
using LightJson.Serialization;
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
        [Command("restart")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Restart(CommandContext ctx)
        {
            var getusersEmbed = new DiscordEmbedBuilder
            {
                Description = "Все новые пользователи были принудительно добавлены в `usersData.json`.",
                Color = DiscordColor.Gold,
            };

            var users = ctx.Guild.Members;

            var sunCount = 0;
            var file = File.CreateText(Environment.CurrentDirectory + "\\Data\\" + "\\usersData.json");
            
            var usersData = new JsonObject();
            var writer = new JsonWriter(file, true);

            foreach (var item in users)
            {
                var tempUser = item.Value.Username;
                var tempId = item.Key;

                usersData.Add(tempUser, new JsonArray()
                    .Add(tempId.ToString())
                    .Add(sunCount)
                .ToString(true));
            }

            foreach (var item in usersData)
            {
                System.Console.WriteLine(item + "\n");
            }

            writer.Write(usersData);
            file.Close();

            await ctx.Channel.SendMessageAsync(embed: getusersEmbed).ConfigureAwait(false);
        }

        [Command("find")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Find(CommandContext ctx, DiscordMember user)
        {
            //var profileInfo = JsonValue.Parse(users)[user.Username].AsJsonArray;
            //System.Console.WriteLine(profileInfo);

            var reader = JsonReader.Parse(File.ReadAllText(Environment.CurrentDirectory + "\\Data\\" + "\\usersData.json"));

            var cached = new Dictionary<string, string>();

            var users =  new JsonObject()
	            .Add("menu", new JsonArray()
		        .Add("home")
		        .Add("projects")
		        .Add("about")
	            )
	            .ToString(true);

            var menu = JsonValue.Parse(reader)["menu"].AsJsonArray;
            System.Console.WriteLine(menu);

            /*foreach (var item in users)
            {
                userInfo = userInfo + profileInfo[item];
            }*/
            System.Console.WriteLine(reader.ToString());

            //await ctx.Channel.SendMessageAsync($"Информация о пользователе {user.Mention}: {userInfo}").ConfigureAwait(false);
        }

        [Command("load")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Load(CommandContext ctx, DiscordMember user)
        {
            //var path = Environment.CurrentDirectory + "\\usersData.json";

            var path = File.CreateText(Environment.CurrentDirectory + "\\Data\\" + "\\usersData.json");
            System.Console.WriteLine(path);

        }
    }
}