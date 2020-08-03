using Sunflower.Bot.Attributes;
using Newtonsoft.Json;
using System.Text;
using DSharpPlus;
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
using Microsoft.EntityFrameworkCore;
using Sunflower.Models;
using Sunflower.Context;

namespace Sunflower.Bot.Commands
{
    public class DataCommands : BaseCommandModule
    {
        string path = Environment.CurrentDirectory + "\\Data\\" + "Guild\\";

        [Command("migrate")]
        [Hidden]
        public async Task Migrate(CommandContext ctx)
        {
            await using SunflowerUsersContext users = new SunflowerUsersContext();

            if (users.Database.GetPendingMigrationsAsync().Result.Any())
            {
                await users.Database.MigrateAsync();
            }

            await ctx.Channel.SendMessageAsync("sqlite migration complete");
        }

        [Command("createdb")]
        [Hidden]
        public async Task CreateDB(CommandContext ctx)
        {
            var users = ctx.Guild.Members;

            foreach (var item in users)
            {
                var user = new Profile();
                user.MemberId = item.Key;
                user.MemberUsername = item.Value.Username;
                user.MemberSunCount = 0;
                user.DailyCooldown = DateTime.Now.Date;

                using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
                {
                    usersContext.Users.Add(user);
                    await usersContext.SaveChangesAsync();
                }
            }

            using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
            {
                var count = usersContext.Users.Count();

                await ctx.Channel.SendMessageAsync($"{count} users saved");
            }

        }

        [Command("servers")]
        [Description("Выводит список всех серверов на которых присутствует бот")]
        [RequirePermissions(Permissions.Administrator)]
        [Hidden]
        public async Task Servers(CommandContext ctx)
        {
            var serversEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Gold,
            };

            var listName = String.Empty;
            var listID = String.Empty;
            var listMembers = String.Empty;

            foreach (var item in ctx.Client.Guilds.Values)
            {
                listName = listName + string.Join(" ", item.Name  + "\n");
                listID = listID + string.Join(" ", item.Id + "\n");
                listMembers = listMembers + string.Join(" ", item.MemberCount + "\n");
            }

            serversEmbed.WithAuthor("Список серверов:", null, ctx.Guild.CurrentMember.AvatarUrl);
            serversEmbed.AddField("Название:", listName, true);
            serversEmbed.AddField("ID:", listID, true);
            serversEmbed.AddField("Кол-во:", listMembers, true);
            serversEmbed.WithTimestamp(DateTime.Now);

            await ctx.Channel.SendMessageAsync(embed: serversEmbed).ConfigureAwait(false);
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