using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Sunflower.DAL.Context;
using Sunflower.DAL.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sunflower.Bot.Commands
{
    public class DataCommands : BaseCommandModule
    {
        [Command("migrate")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        [Hidden]
        public async Task Migrate(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();

            await using DatabaseContext users = new DatabaseContext();

            if (users.Database.GetPendingMigrationsAsync().Result.Any())
            {
                await users.Database.MigrateAsync();
            }

            var migrateEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold).WithDescription("Миграция SQLite успешно завершена!");
            await ctx.Channel.SendMessageAsync(embed: migrateEmbed).ConfigureAwait(false);
        }

        [Command("createdb")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        [Hidden]
        public async Task CreateDB(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();

            using (DatabaseContext usersContext = new DatabaseContext())
            {
                foreach (var item in ctx.Guild.Members)
                {
                    if (usersContext.UserProfiles.Any(x => (x.MemberId == item.Key) && x.GuildId == item.Value.Guild.Id) == true)
                    {
                        continue;
                    }
                    else
                    {
                        var user = new Profile()
                        {
                            GuildId = ctx.Guild.Id,
                            MemberId = item.Key,
                            MemberUsername = item.Value.Username,
                            MemberSunCount = 0,
                            DailyCooldown = DateTime.Now
                        };

                        usersContext.UserProfiles.Add(user);
                        await usersContext.SaveChangesAsync();
                    }
                    
                }
            }
            var createDBEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold).WithDescription("Принудительное сохранение пользователей завершено!");

            await ctx.Channel.SendMessageAsync(embed: createDBEmbed).ConfigureAwait(false);
        }

        [Command("userinfo")]
        [Description("Информация о пользователе")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Userinfo(CommandContext ctx, [Description("Пользователь, информацию о котором хотите посмотреть")] DiscordMember user)
        {
            var userinfoEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold
            };

            using DatabaseContext usersContext = new DatabaseContext();
            foreach (var item in usersContext.UserProfiles)
            {
                if (item.MemberId == user.Id && item.GuildId == user.Guild.Id)
                {
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

                    var roles = string.Join(" ", user.Roles.OrderByDescending(x => x.Position).Select(x => $"{x.Mention}"));

                    if (roles != String.Empty)
                    {
                        userinfoEmbed.AddField("Роли:", roles);
                    }

                    userinfoEmbed.WithThumbnail(user.AvatarUrl, 500, 500);
                    userinfoEmbed.WithTimestamp(DateTime.Now);

                    await ctx.Channel.SendMessageAsync(embed: userinfoEmbed).ConfigureAwait(false);
                }
            }

        }
    }
}