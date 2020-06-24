using SunflowerBot.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Commands​Next.Converters;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunflowerBot.Commands
{
    public class FlowerCommands : BaseCommandModule
    {
        [Command("sunny")]
        [Description("Получение роли для уведомлений о начале **солнечных** эвентов")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Sunny(CommandContext ctx)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .sunny пользователем {ctx.User.Username}");
            var role = ctx.Guild.GetRole(724675912204812335);

            var interactivity = ctx.Client.GetInteractivity();

            var accept = DiscordEmoji.FromName(ctx.Client, ":sunflower:");

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Хочешь получать уведомления о предстоящих солнечных эвентах?\n",
                Description = $":sunflower: Выбери эмодзи для получения роли.\n\nДанная роль позволяет разрешать упоминать вас ({role.Mention}), во время различных событий, для получения солнышек и других наград.\n\nПосле выбора эмодзи сообщение удалится и вы получите роль. Чтобы отписаться от рассылки наберите команду вновь и отреагируюте эмодзи.",
                Color = DiscordColor.Gold,
            };

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);
            Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат {joinMessage.Id}");

            await joinMessage.CreateReactionAsync(accept).ConfigureAwait(false);   

            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == joinMessage && 
                x.User == ctx.User &&
                (x.Emoji == accept)).ConfigureAwait(false);

            if (reactionResult.Result.Emoji == accept)
            {
                await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
                Console.WriteLine($"[{DateTime.Now}] [Role Log] Добавление роли {role.Id} пользователю {ctx.User.Username}");

                // await joinMessage.DeleteReactionAsync(accept, ctx.User).ConfigureAwait(false);
            }           
        }    

        [Command("give")]
        [Description("Создаёт эвент для выдачи солнышек пользователю, первому написавшему `.confirm`")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task Give(CommandContext ctx, [Description("Количество солнышек")]int sunCount)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .give пользователем {ctx.User.Username}");
            Random rnd = new Random();
            var interactivity = ctx.Client.GetInteractivity();

            var giveawayEmbed = new DiscordEmbedBuilder
            {
                Title = "Получение солнышек",
                Description = $"Напишите первым `.confirm` и получите {sunCount} :sunny: солнышек!",
                Color = DiscordColor.Gold
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: giveawayEmbed).ConfigureAwait(false);
            Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат {pollMessage.Id}");

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);

            while (true)
            {
                if (message.Result.Content == ".confirm")
                {
                    var user = message.Result.Author;

                    var giveawayEndEmbed = new DiscordEmbedBuilder
                    {
                        Description = $"{user.Mention} успевает первым забрать солнышки!",
                        Color = DiscordColor.Gold,
                    };

                    var joinMessage = await ctx.Channel.SendMessageAsync(embed: giveawayEndEmbed).ConfigureAwait(false);
                    Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат {joinMessage.Id}");

                    break;
                }
                else if (message.Result.Content != ".confirm")
                {
                    message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
                }
            }
        }

        [Command("info")]
        [Description("Информация и ссылки")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Info(CommandContext ctx)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .info пользователем {ctx.User.Username}");
            var infoEmbed = new DiscordEmbedBuilder
            {
                Title = "Спасибо за то, что находитесь здесь!",
                Description = $"Просто ссылки на простые страницы, ок:\n\nTwitter: https://twitter.com/aniv1re\nArtStation: https://artstation.com/aniv1re\nTwitch: https://twitch.tv/anivire_\n\nПерманентная ссылка-приглашение на сервер:\nDiscord: https://discord.gg/6YpDYKu",
                Color = DiscordColor.Gold,
            };
            infoEmbed.WithThumbnail("https://i.imgur.com/GFBXBoz.jpg", 1000, 500);

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: infoEmbed).ConfigureAwait(false);
            Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат: {joinMessage.Id}");
        }  

        
        [Command("patreon")]
        [Description("Платный контент")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Patreon(CommandContext ctx)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .patreon пользователем {ctx.User.Username}");
            var patreonEmbed = new DiscordEmbedBuilder
            {
                Title = "Уникальный контент для платных подписчиков!",
                Description = $"У вас есть возможно подписаться за 1$ или 3$ на Patreon, чтобы получить доступ к уникальному контенту, секретным эвентам и преимуществам в Discord.\n\nPatreon: https://www.patreon.com/aniv1re",
                Color = DiscordColor.Gold,
            };
            patreonEmbed.WithThumbnail("https://cahoicatam.com/images/patreon-logo-png-white-1.png", 1000, 500);
            patreonEmbed.WithFooter("Все преимущества от тиров Patreon'а даёт также платная подписка на Twitch");

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: patreonEmbed).ConfigureAwait(false);
            Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат: {joinMessage.Id}");
        }   

        [Command("thief")]
        [Description("Создаёт эвент для ограбления пользователя")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Thief(CommandContext ctx, DiscordMember user)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .thief пользователем {ctx.User.Username}");
            Random rnd = new Random();
            var interactivity = ctx.Client.GetInteractivity();

            var thiefEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold
            };

            var statusBar = string.Empty;

            var small = DiscordEmoji.FromName(ctx.Client, ":small:");
            var fullLeft = DiscordEmoji.FromName(ctx.Client, ":fullLeft:");
            var full = DiscordEmoji.FromName(ctx.Client, ":full:");
            var fullRight = DiscordEmoji.FromName(ctx.Client, ":fullRight:");
            var empty = DiscordEmoji.FromName(ctx.Client, ":empty:");
            var emptyRight = DiscordEmoji.FromName(ctx.Client, ":emptyRight:");

            var randomPercent = rnd.Next(1, 100);

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
            else if (randomPercent <= 90)
            {
                statusBar = fullLeft + full + full + fullRight + empty + emptyRight;
            }
            else if (randomPercent > 90)
            {
                statusBar = fullLeft + full + full + full + fullRight + emptyRight;
            }

            if (user == ctx.User)
            {
                thiefEmbed.WithDescription("Ты чево наделал...");
                thiefEmbed.WithImageUrl("https://answers.ea.com/ea/attachments/ea/battlefield-v-game-information-ru/1635/1/1785C7EE-5715-48CA-A2FC-16479F84D644.jpeg");
            }
            else
            {
                thiefEmbed.WithAuthor(ctx.User.Username + $" начинает грабить " + user.Username, null, ctx.User.AvatarUrl);
                thiefEmbed.WithDescription($"Его шанс на успех:\t{statusBar} {randomPercent}%");
            }

            var thiefMessage = await ctx.Channel.SendMessageAsync(embed: thiefEmbed).ConfigureAwait(false);
            Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат {thiefMessage.Id}");
        }
    }
}