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
    }
}