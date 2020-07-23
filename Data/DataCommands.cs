using Sunflower.Bot.Attributes;
using Sunflower.Bot.Data;
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

namespace Sunflower.Bot.Commands
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
        public async Task Load(CommandContext ctx)
        {
            var dataTable = DataLoad.LoadDB(path, ctx.Guild.Id);

            var loadDataEmbed = new DiscordEmbedBuilder
            {
                Description = $"Загрузка пользователей из `{ctx.Guild.Id}.json`.",
                Color = DiscordColor.Gold,
            };

            loadDataEmbed.AddField("Пользователи:", string.Join(" ", ctx.Guild.Members.Select(x => $"{x.Value.Mention}")));

            await ctx.Channel.SendMessageAsync(embed: loadDataEmbed).ConfigureAwait(false);
        }

        [Command("userinfo")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Userinfo(CommandContext ctx)
        {
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(path + $"{ctx.Guild.Id}.json"));
            DataTable dataTable = dataSet.Tables["Table1"];

            var userinfoEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["discordId"].ToString() == ctx.Member.Id.ToString())
                {
                    var roles = ctx.Member.Roles;

                    userinfoEmbed.WithAuthor($"Пользователь {ctx.Member.Username}");
                    userinfoEmbed.AddField("Количество солнц:", $":sunny: {row["sunCount"]}", true);
                    userinfoEmbed.AddField("Присоединился к серверу:", ctx.Member.JoinedAt.DateTime.ToString(), true);
                    userinfoEmbed.AddField("Роли:", string.Join(" ", roles.OrderByDescending(x => x.Position).Select(x => $"{x.Mention}")));
                    userinfoEmbed.WithThumbnail(ctx.Member.AvatarUrl, 500, 500);

                    await ctx.Channel.SendMessageAsync(embed: userinfoEmbed).ConfigureAwait(false);
                }
            }

        }
    }
}