using SunflowerBot.Attributes;
using SunflowerBot.Data;
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
using System.Data;
using System.Threading.Tasks;

namespace SunflowerBot.Commands
{
    public class DataCommands : BaseCommandModule
    {
        string path = Environment.CurrentDirectory + "\\Data\\" + "Guild\\";

        [Command("createdata")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Createdata(CommandContext ctx)
        {
            path = Environment.CurrentDirectory + "\\Data\\" + "Guild\\" + $"{ctx.Guild.Id}.json";
            var users = ctx.Guild.Members;

            var getusersEmbed = new DiscordEmbedBuilder
            {
                Description = $"Пользователи были принудительно добавлены в `{ctx.Guild.Id}.json`.",
                Color = DiscordColor.Gold,
            };

            DataCreate.CreateDB(path, users);

            await ctx.Channel.SendMessageAsync(embed: getusersEmbed).ConfigureAwait(false);
        }

        [Command("load")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Load(CommandContext ctx, [Description("ID необходимого сервера")]ulong guild)
        {
            var users = String.Empty;

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(path + $"{guild}.json"));
            DataTable dataTable = dataSet.Tables["Table1"];
            Console.WriteLine(dataSet);

            foreach (DataRow row in dataTable.Rows)
            {
                users = users + string.Join(" ", row["username"] + " ");
            }

            var loadDataEmbed = new DiscordEmbedBuilder
            {
                Description = $"Загрузка пользователей из `{ctx.Guild.Name}.json`.\n\n"
                    + $"Количество пользователей на сервере - `\n`"
                    + $"{users}",
                Color = DiscordColor.Gold,
            };

            await ctx.Channel.SendMessageAsync(embed: loadDataEmbed).ConfigureAwait(false);
        }
    }
}