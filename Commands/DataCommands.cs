using SunflowerBot.Attributes;
using LightJson;
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
        [Command("reload")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Reload(CommandContext ctx)
        {
            var users = ctx.Guild.Members;

            var sunCount = 0;

            foreach (var item in users)
            {
                var tempUser = item.Value.Username;
                var tempId = item.Key;

                var profile = new JsonObject()
	                .Add(tempUser, new JsonArray()
                        .Add(tempUser)
		                .Add(tempId.ToString())
		                .Add(sunCount)
	                )
	                .ToString(true);

                var user = JsonValue.Parse(profile)[tempUser].AsJsonArray;

                foreach (var i in user)
                {
                    Console.WriteLine(i.ToString());
                }

                var path = Environment.CurrentDirectory + "\\usersData.json";

                File.AppendAllText(path, profile + Environment.NewLine);
            }

            await ctx.Channel.SendMessageAsync("База данных была перезагружена.").ConfigureAwait(false);
        }
    }
}