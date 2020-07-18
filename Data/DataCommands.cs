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
            path = Environment.CurrentDirectory + "\\Data\\" + "Guild\\" + $"{ctx.Guild.Name}.json";

            var getusersEmbed = new DiscordEmbedBuilder
            {
                Description = $"Пользователи были принудительно добавлены в `{ctx.Guild.Name}.json`.",
                Color = DiscordColor.Gold,
            };

            DataSet usersList = new DataSet("usersList");
            usersList.Namespace = "DiscordUsers";

            DataTable table = new DataTable();
            DataColumn idColumn = new DataColumn("id", typeof(int));
            idColumn.AutoIncrement = true;

            DataColumn discordIdColumn = new DataColumn("discordId", typeof(ulong));
            DataColumn usernameColumn = new DataColumn("username", typeof(string));
            DataColumn sunCountColumn = new DataColumn("sunCount", typeof(int));

            table.Columns.Add(idColumn);
            table.Columns.Add(discordIdColumn);
            table.Columns.Add(usernameColumn);
            table.Columns.Add(sunCountColumn);

            usersList.Tables.Add(table);

            var users = ctx.Guild.Members;

            foreach (var item in users)
            {
                DataRow newRow = table.NewRow();
                newRow["discordId"] = item.Key;
                newRow["username"] = item.Value.Username;
                newRow["sunCount"] = 0;

                table.Rows.Add(newRow);
            }

            usersList.AcceptChanges();

            File.AppendAllText(path, JsonConvert.SerializeObject(usersList, Formatting.Indented));

            await ctx.Channel.SendMessageAsync(embed: getusersEmbed).ConfigureAwait(false);
        }

        [Command("load")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Load(CommandContext ctx, params string[] guild)
        {
            var users = String.Empty;
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(path + $"{ctx.Guild.Name}.json"));
            System.Console.WriteLine(path + $"{ctx.Guild.Name}.json");

            DataTable dataTable = dataSet.Tables["Table1"];

            foreach (DataRow row in dataTable.Rows)
            {
                users = users + string.Join(" ", row["username"] + " ");
            }

            var loadDataEmbed = new DiscordEmbedBuilder
            {
                Description = $"Загрузка пользователей из `{ctx.Guild.Name}.json`.\n\n"
                    + $"Количество пользователей на сервере - `{dataTable.Rows.Count}\n`"
                    + $"{users}",
                Color = DiscordColor.Gold,
            };

            

            await ctx.Channel.SendMessageAsync(embed: loadDataEmbed).ConfigureAwait(false);
        }
    }
}