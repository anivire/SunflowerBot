using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Sunflower.DAL.Context;
using Sunflower.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sunflower.Bot.Commands
{
    public class SunflowerCommands : BaseCommandModule
    {
        [Command("sunny")]
        [Description("Получение роли для уведомлений о начале **солнечных** эвентов")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        public async Task Sunny(CommandContext ctx, [RemainingText, Description("Роль, которая необходима для уведомлений о начале эвентов")] DiscordRole role)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var accept = DiscordEmoji.FromName(ctx.Client, ":sunflower:");

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Хочешь получать уведомления о предстоящих солнечных эвентах?\n",
                Description = $":sunflower: Выбери эмодзи для получения роли.\n\nДанная роль позволяет разрешать упоминать вас ({role.Mention}), во время различных событий, для получения солнышек и других наград.\n\nПосле выбора эмодзи сообщение удалится и вы получите роль. Чтобы отписаться от рассылки наберите команду вновь и отреагируюте эмодзи.",
                Color = DiscordColor.Gold
            };

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            await joinMessage.CreateReactionAsync(accept).ConfigureAwait(false);

            using DatabaseContext sunny = new DatabaseContext();
            if (sunny.SunnyMessage.Any(x => x.GuildId == ctx.Guild.Id) == false)
            {
                var sunnyMes = new MessageReaction()
                {
                    GuildId = ctx.Guild.Id,
                    RoleId = role.Id,
                    MessageId = joinMessage.Id
                };

                sunny.SunnyMessage.Add(sunnyMes);
                await sunny.SaveChangesAsync();
            }
            else
            {
                sunny.SunnyMessage.Single(x => x.GuildId == ctx.Guild.Id).MessageId = joinMessage.Id;
                sunny.SunnyMessage.Single(x => x.GuildId == ctx.Guild.Id).RoleId = role.Id;
                await sunny.SaveChangesAsync();
            }
        }

        [Command("daily")]
        [Aliases("d", "reward", "r")]
        [Description("Получение ежедневной награды")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Daily(CommandContext ctx)
        {
            using DatabaseContext usersContext = new DatabaseContext();
            foreach (var item in usersContext.UserProfiles)
            {
                if (item.MemberId == ctx.User.Id && item.GuildId == ctx.Guild.Id)
                {
                    if (DateTime.Compare(DateTime.Now, item.DailyCooldown) > 0)
                    {
                        var reward = new Random().Next(25, 50);
                        var dailyEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold);

                        dailyEmbed.WithAuthor("Ежедневная награда", null, ctx.User.AvatarUrl);
                        dailyEmbed.AddField("Солнышки", $"+{reward} :sunny:", true);
                        //dailyEmbed.AddField("Растения", "`empty`", true);
                        //dailyEmbed.AddField("Семена", "`empty`", true);
                        dailyEmbed.WithTimestamp(DateTime.Now);

                        item.MemberSunCount += reward;
                        item.DailyCooldown = DateTime.Now.AddHours(12);
                        await usersContext.SaveChangesAsync();

                        await ctx.Channel.SendMessageAsync(embed: dailyEmbed).ConfigureAwait(false);
                    }
                    else
                    {
                        var temp = item.DailyCooldown - DateTime.Now;

                        var dailyEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold).WithDescription("Вы уже получили ежедневную награду.\n" +
                            $"Следующая награда будет доступна через **{temp.Hours}:{temp.Minutes}:{temp.Seconds}**");
                        await ctx.Channel.SendMessageAsync(embed: dailyEmbed).ConfigureAwait(false);
                    }
                }
            }
        }

        [Command("rob")]
        [Description("Создаёт эвент для ограбления пользователя")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Rob(CommandContext ctx, [Description("Пользователь, для попытки ограбления (`@username`)")] DiscordMember user)
        {
            Random rnd = new Random();
            
            var statusBar = string.Empty;
            var small = DiscordEmoji.FromName(ctx.Client, ":small:");
            var fullLeft = DiscordEmoji.FromName(ctx.Client, ":fullLeft:");
            var full = DiscordEmoji.FromName(ctx.Client, ":full:");
            var fullRight = DiscordEmoji.FromName(ctx.Client, ":fullRight:");
            var fullRightEnd = DiscordEmoji.FromName(ctx.Client, ":fullRightEnd:");
            var empty = DiscordEmoji.FromName(ctx.Client, ":empty:");
            var emptyRight = DiscordEmoji.FromName(ctx.Client, ":emptyRight:");
            var randomPercent = rnd.Next(0, 101);

            var thiefEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold);

            if (randomPercent <= 15)
            {
                statusBar = small + empty + empty + empty + empty + emptyRight;
            }
            else if (randomPercent <= 30)
            {
                statusBar = fullLeft + fullRight + empty + empty + empty + emptyRight;
            }
            else if (randomPercent <= 50)
            {
                statusBar = fullLeft + full + fullRight + empty + empty + emptyRight;
            }
            else if (randomPercent < 90)
            {
                statusBar = fullLeft + full + full + fullRight + empty + emptyRight;
            }
            else if (randomPercent >= 90 && randomPercent < 100)
            {
                statusBar = fullLeft + full + full + full + fullRight + emptyRight;
            }
            else if (randomPercent >= 100)
            {
                statusBar = fullLeft + full + full + full + full + fullRightEnd;
            }

            if (user == ctx.User)
            {
                thiefEmbed.WithImageUrl("https://answers.ea.com/ea/attachments/ea/battlefield-v-game-information-ru/1635/1/1785C7EE-5715-48CA-A2FC-16479F84D644.jpeg");
                await ctx.Channel.SendMessageAsync(embed: thiefEmbed).ConfigureAwait(false);
            }
            else
            {
                thiefEmbed.WithAuthor(ctx.User.Username + $" начинает грабить " + user.Username, null, ctx.User.AvatarUrl);
                thiefEmbed.WithDescription($"Шанс на успех:\t{statusBar} {randomPercent}%");

                using (DatabaseContext usersContext = new DatabaseContext())
                {
                    
                }
 
                await ctx.Channel.SendMessageAsync(embed: thiefEmbed).ConfigureAwait(false);
            }
        }

        [Command("give")]
        [Description("Создаёт эвент для выдачи солнышек пользователю, первому написавшему `.confirm`")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task Give(CommandContext ctx, [Description("Количество солнышек")]int sunCount, [Description("*Необязательно; сообщение для вывода")]params string[] content)
        {
            Random rnd = new Random();
            var interactivity = ctx.Client.GetInteractivity();

            var giveawayEmbed = new DiscordEmbedBuilder
            {
                Title = "Получение солнышек",
                Color = DiscordColor.Gold
            };

            if (sunCount <= 0)
            {
                await ctx.Channel.SendMessageAsync(embed: giveawayEmbed.WithDescription("Вы ввели неправильное количество солнц для выдачи.")).ConfigureAwait(false);
                return; 
            }

            if (string.Join(" ", content) == string.Empty)
            {
                giveawayEmbed.WithDescription($"Напишите первым `.confirm` и получите {sunCount} :sunny: солнышек!");
            }
            else
            {
                giveawayEmbed.WithDescription($"Напишите первым `.confirm` и получите {sunCount} :sunny: солнышек!\n\n{string.Join(" ", content)}");
            }

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: giveawayEmbed).ConfigureAwait(false);

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            while (true)
            {
                if (message.Result.Content == ".confirm")
                {
                    var user = message.Result.Author;

                    var giveawayEndEmbed = new DiscordEmbedBuilder
                    {
                        Description = $"{user.Mention} успевает первым забрать солнышки!",
                        Color = DiscordColor.Gold
                    };

                    using (DatabaseContext usersContext = new DatabaseContext())
                    {
                        foreach (var item in usersContext.UserProfiles)
                        {
                            if (item.MemberId == user.Id)
                            {
                                item.MemberSunCount += sunCount;

                                await usersContext.SaveChangesAsync();
                            }
                        }
                    }

                    var joinMessage = await ctx.Channel.SendMessageAsync(embed: giveawayEndEmbed).ConfigureAwait(false);

                    break;
                }
                else if (message.Result.Content != ".confirm")
                {
                    message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                }
            }
        }
    }
}