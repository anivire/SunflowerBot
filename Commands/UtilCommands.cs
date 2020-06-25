using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunflowerBot.Commands
{
    public class UtilCommands : BaseCommandModule
    {
        [Command("del")]
        [Description("Удаление сообщений")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task Del(CommandContext ctx, [Description("Количество сообщений для удаления")]int numberMessages)
        {
            var currentMsg = await ctx.Channel.GetMessagesAsync(numberMessages + 1);
            await ctx.Channel.DeleteMessagesAsync(currentMsg, "Сообщение удалено с помощью команды `.clear`");

            var delEmbed = new DiscordEmbedBuilder
            {
                 Color = DiscordColor.Gold
            };

            delEmbed.WithDescription($"Удалено {numberMessages} сообщений");

            var clearMessage = await ctx.Channel.SendMessageAsync(embed: delEmbed).ConfigureAwait(false);
        }

        [Command("deln")]
        [Description("Удаление сообщений без вывода форматированного текста")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task Deln(CommandContext ctx, [Description("Количество сообщений для удаления")]int numberMessages)
        {
            var currentMsg = await ctx.Channel.GetMessagesAsync(numberMessages + 1);
            await ctx.Channel.DeleteMessagesAsync(currentMsg, "Сообщение удалено с помощью команды `.clear`");
        }

        [Command("roll")]
        [Description("Получение случайного числа")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Roll(CommandContext ctx, [Description("Минимальное число")]int min, [Description("Максимальное число")]int max)
        {
            var rnd = new Random();
            
            var rollEmbed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Gold
                };
            
            if (max >= 1 && max > min)
            {
                rollEmbed.WithDescription($"🎲 Случайное число: {rnd.Next(min, max + 1)}");

                var rollMessage = await ctx.Channel.SendMessageAsync(embed: rollEmbed).ConfigureAwait(false);
            }
            else if (max <= 1)   
            {
                rollEmbed.WithDescription("Некорректно заданное число");

                var rollMessage = await ctx.Channel.SendMessageAsync(embed: rollEmbed).ConfigureAwait(false);
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

        [Command("rob")]
        [Description("Создаёт эвент для ограбления пользователя")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Rob(CommandContext ctx, DiscordMember user)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .thief пользователем {ctx.User.Username}");

            Random rnd = new Random();
            var interactivity = ctx.Client.GetInteractivity();
            
            DiscordMember sunflowerGod = await ctx.Channel.Guild.GetMemberAsync(673527202029109258);

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

        [Command("suggest")]
        [Description("Голосование за тему арта месяца")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Twitch Sub", "Patreon Tier 3$")]
        public async Task Suggest(CommandContext ctx, params string[] content)
        {
            Console.WriteLine($"[{DateTime.Now}] [Command Log] Использована команда .suggest пользователем {ctx.User.Username}");

            var suggestEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            if (string.Join(" ", content) == string.Empty)
            {
                suggestEmbed.WithDescription("Вы не указали тему предложения");

                var joinMessage = await ctx.Channel.SendMessageAsync(embed: suggestEmbed).ConfigureAwait(false);
                Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат {joinMessage.Id}");
            }
            else
            {
                suggestEmbed.WithTitle("Предложение темы для арта месяца");
                suggestEmbed.WithDescription($"{ctx.User.Mention}: {string.Join(" ", content)}");

                var joinMessage = await ctx.Channel.SendMessageAsync(embed: suggestEmbed).ConfigureAwait(false);
                Console.WriteLine($"[{DateTime.Now}] [Chat Log] Отправлено сообщение в чат {joinMessage.Id}");

                await joinMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":upVote:")).ConfigureAwait(false);
                await joinMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":downVote:")).ConfigureAwait(false);
            }
        }
    }
}