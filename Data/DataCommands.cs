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
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Createdata(CommandContext ctx)
        {
            path = Environment.CurrentDirectory + "\\Data\\" + "Guild\\" + $"{ctx.Guild.Id}.json";
            var users = ctx.Guild.Members;

            var getusersEmbed = new DiscordEmbedBuilder
            {
                Description = $"Пользователи были добавлены в `{ctx.Guild.Id}.json`.",
                Color = DiscordColor.Gold,
            };

            DataCreate.CreateDB(path, users);

            await ctx.Channel.SendMessageAsync(embed: getusersEmbed).ConfigureAwait(false);
        }

        [Command("load")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Load(CommandContext ctx)
        {
            //var dataTable = DataLoad.LoadDB(path, ctx.Guild.Id);

            var loadDataEmbed = new DiscordEmbedBuilder
            {
                Description = $"Загрузка пользователей из `{ctx.Guild.Id}.json`.",
                Color = DiscordColor.Gold,
            };

            loadDataEmbed.AddField("Пользователи:", string.Join(" ", ctx.Guild.Members.Select(x => $"{x.Value.Mention}")));

            await ctx.Channel.SendMessageAsync(embed: loadDataEmbed).ConfigureAwait(false);
        }

        [Command("userinfo")]
        [Description("Информация о пользователе")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Userinfo(CommandContext ctx, [Description("Пользователь, информацию о котором хотите посмотреть")] DiscordMember user)
        {
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(path + $"{ctx.Guild.Id}.json"));
            DataTable dataTable = dataSet.Tables["Table1"];

            var userinfoEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            /*foreach (DataRow row in dataTable.Rows)
            {
                if (row["discordId"].ToString() == ctx.Member.Id.ToString())
                {
                    var roles = ctx.Member.Roles;

                    var botCheck = String.Empty;
                    if (ctx.Member.IsBot)
                    {
                        botCheck = " - Bot";
                    }

                    userinfoEmbed.WithTitle($"Пользователь {ctx.Member.Username}#{ctx.Member.Discriminator}" + botCheck);
                    userinfoEmbed.AddField("Количество солнц:", $":sunny: {row["sunCount"]}", true);
                    userinfoEmbed.AddField("Текущий ник:", ctx.Member.DisplayName, true);
                    userinfoEmbed.AddField("Зашёл на сервер:", ctx.Member.JoinedAt.DateTime.ToShortDateString(), true);
                    userinfoEmbed.AddField("Роли:", string.Join(" ", roles.OrderByDescending(x => x.Position).Select(x => $"{x.Mention}")));
                    userinfoEmbed.WithThumbnail(ctx.Member.AvatarUrl, 500, 500);

                    await ctx.Channel.SendMessageAsync(embed: userinfoEmbed).ConfigureAwait(false);
                }
            }*/

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["discordId"].ToString() == user.Id.ToString())
                {
                    var roles = user.Roles;

                    var botCheck = String.Empty;
                    if (user.IsBot)
                    {
                        botCheck = " [Bot]";
                    }

                    userinfoEmbed.WithTitle($"Пользователь {user.Username}#{user.Discriminator}" + botCheck);
                    userinfoEmbed.AddField("Количество солнц:", $":sunny: {row["sunCount"]}", true);
                    userinfoEmbed.AddField("Текущий ник:", user.DisplayName, true);
                    userinfoEmbed.AddField("Зашёл на сервер:", user.JoinedAt.DateTime.ToShortDateString(), true);
                    userinfoEmbed.AddField("Роли:", string.Join(" ", roles.OrderByDescending(x => x.Position).Select(x => $"{x.Mention}")));
                    userinfoEmbed.WithThumbnail(user.AvatarUrl, 500, 500);

                    await ctx.Channel.SendMessageAsync(embed: userinfoEmbed).ConfigureAwait(false);
                }
            }

        }

        [Command("list")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task List(CommandContext ctx)
        {
            DirectoryInfo dir = new DirectoryInfo(@"C:\Users\anivire\source\repos\Sunflower\Sunflower\bin\Debug\netcoreapp3.1\Data\Guild");
            var guilds = String.Empty;
            var i = 1;

            foreach (var item in dir.GetFiles())
            {
                guilds = guilds + string.Join(" ", $"{i}. `{item.Name}`\n");
                i++;
            }

            var loadDataEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            loadDataEmbed.WithDescription("Доступные сервера:\n\n" + guilds);

            await ctx.Channel.SendMessageAsync(embed: loadDataEmbed).ConfigureAwait(false);
        }
    }
}