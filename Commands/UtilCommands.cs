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
        [RequireRoles(RoleCheckMode.All)]
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
        [Description("Получение роли для уведомлений о начале **солнечных** эвентов")]
        [RequireRoles(RoleCheckMode.All)]
        public async Task Info(CommandContext ctx)
        {
            var infoEmbed = new DiscordEmbedBuilder
            {
                Title = "Спасибо за то, что находитесь здесь!",
                Description = $"Просто ссылки на простые страницы, ок:\n\nTwitter: https://twitter.com/aniv1re\nArtStation: https://artstation.com/aniv1re\nTwitch: https://twitch.tv/anivire_\n\nПерманентная ссылка-приглашение на сервер:\nDiscord: https://discord.gg/6YpDYKu",
                Color = DiscordColor.Gold,
            };
            infoEmbed.WithThumbnail("https://i.imgur.com/GFBXBoz.jpg", 1000, 500);

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: infoEmbed).ConfigureAwait(false);   
        }   

        [Command("rockpaperscissors")]
        [Description("Игра - камень, ножницы, бумага")]
        [RequireRoles(RoleCheckMode.All)]
        public async Task RockPaperScissors(CommandContext ctx, string item)
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Камень, ножницы, бумага",
                Description = "",
                Color = DiscordColor.Gold
            };
            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);
        }
    }
}