using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Sunflower.Context;
using Sunflower.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sunflower.Bot.Commands
{
    public class DataCommands : BaseCommandModule
    {
        [Command("migrate")]
        [Hidden]
        public async Task Migrate(CommandContext ctx)
        {
            await using SunflowerUsersContext users = new SunflowerUsersContext();

            if (users.Database.GetPendingMigrationsAsync().Result.Any())
            {
                await users.Database.MigrateAsync();
            }

            await ctx.Channel.SendMessageAsync("Миграция SQLite успешно завершена!");
        }

        [Command("createdb")]
        [Hidden]
        public async Task CreateDB(CommandContext ctx)
        {
            var temp = 0;
            var check = false;

            await ctx.Channel.TriggerTypingAsync();

            using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
            {
                foreach (var item in ctx.Guild.Members)
                {
                    try
                    {
                        foreach (var itemEX in usersContext.UserProfiles)
                        {
                            if (itemEX.MemberId == item.Key)
                            {
                                check = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        check = true;
                    }

                    if (check == true)
                    {
                        temp++;
                        continue;
                    }
                    else
                    {
                        var user = new Profile()
                        {
                            MemberId = item.Key,
                            MemberUsername = item.Value.Username,
                            MemberSunCount = 0,
                            DailyCooldown = DateTime.Now.Date
                        };

                        usersContext.UserProfiles.Add(user);
                        await usersContext.SaveChangesAsync();
                    }
                }
            }

            await ctx.Channel.SendMessageAsync($"Количество совпадений {temp}");

            await ctx.Channel.SendMessageAsync($"Принудительное сохранение пользователей завершено!");
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
                listName += string.Join(" ", item.Name  + "\n");
                listID += string.Join(" ", item.Id + "\n");
                listMembers += string.Join(" ", item.MemberCount + "\n");
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
            var userinfoEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
            {
                foreach (var item in usersContext.UserProfiles)
                {
                    if (item.MemberId == user.Id)
                    {
                        var roles = user.Roles;

                        var botCheck = String.Empty;
                        if (user.IsBot)
                        {
                            botCheck = " Bot";
                        }
                        userinfoEmbed.WithAuthor($"{user.Username}#{user.Discriminator}" + botCheck, null, user.AvatarUrl);

                        if (!user.IsBot)
                        {
                            userinfoEmbed.AddField("Количество солнц:", $":sunny: {item.MemberSunCount}", true);
                        }
                        
                        userinfoEmbed.AddField("Текущий ник:", user.DisplayName, true);
                        userinfoEmbed.AddField("Зашёл на сервер:", user.JoinedAt.DateTime.ToShortDateString(), true);
                        userinfoEmbed.AddField("Роли:", string.Join(" ", roles.OrderByDescending(x => x.Position).Select(x => $"{x.Mention}")));
                        userinfoEmbed.WithThumbnail(user.AvatarUrl, 500, 500);

                        await ctx.Channel.SendMessageAsync(embed: userinfoEmbed).ConfigureAwait(false);
                    }
                }
            }

        }
    }
}