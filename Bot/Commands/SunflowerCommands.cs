﻿using Sunflower.Bot.Attributes;
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
using System.Threading.Tasks;

namespace Sunflower.Bot.Commands
{
    public class SunflowerCommands : BaseCommandModule
    {
        [Command("sunny")]
        [Description("Получение роли для уведомлений о начале **солнечных** эвентов")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Sunny(CommandContext ctx)
        {
            var role = ctx.Guild.GetRole(720276705071333506);
            var interactivity = ctx.Client.GetInteractivity();
            var accept = DiscordEmoji.FromName(ctx.Client, ":sunflower:");

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Хочешь получать уведомления о предстоящих солнечных эвентах?\n",
                Description = $":sunflower: Выбери эмодзи для получения роли.\n\nДанная роль позволяет разрешать упоминать вас ({role.Mention}), во время различных событий, для получения солнышек и других наград.\n\nПосле выбора эмодзи сообщение удалится и вы получите роль. Чтобы отписаться от рассылки наберите команду вновь и отреагируюте эмодзи.",
                Color = DiscordColor.Gold,
            };

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            await joinMessage.CreateReactionAsync(accept).ConfigureAwait(false);

            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == joinMessage &&
                x.User == ctx.User &&
                (x.Emoji == accept)).ConfigureAwait(false);

            if (reactionResult.Result.Emoji == accept)
            {
                await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);

                // await joinMessage.DeleteReactionAsync(accept, ctx.User).ConfigureAwait(false);*/
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
                Color = DiscordColor.Gold,
            };

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
                        Color = DiscordColor.Gold,
                    };

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