﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sunflower.Bot.Commands
{
    public class MainCommands : BaseCommandModule
    {
        [Group("util")]
        [Aliases("u")]
        [RequireRoles(RoleCheckMode.Any, "Sun Sponsor")]
        [Description("Группа команд для работы с сервером и ботом ")]
        public class UtilCommands : BaseCommandModule
        {
            [Command("del")]
            [Description("Удаление сообщений")]
            public async Task Del(CommandContext ctx, [Description("Количество сообщений для удаления")] int numberMessages)
            {
                var currentMsg = await ctx.Channel.GetMessagesAsync(numberMessages + 1);
                await ctx.Channel.DeleteMessagesAsync(currentMsg, "Сообщение удалено с помощью команды `.clear`");

                var delEmbed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Gold
                };

                delEmbed.WithDescription($"Удалено {numberMessages} сообщений");

                await ctx.Channel.SendMessageAsync(embed: delEmbed).ConfigureAwait(false);
            }

            [Command("dell")]
            [Description("Удаление сообщений без вывода embed")]
            public async Task Deln(CommandContext ctx, [Description("Количество сообщений для удаления")] int numberMessages)
            {
                var currentMsg = await ctx.Channel.GetMessagesAsync(numberMessages + 1);
                await ctx.Channel.DeleteMessagesAsync(currentMsg, "Сообщение удалено с помощью команды `.clear`");
            }

            [Command("act")]
            [Description("Изменение статуса бота")]
            public async Task Activity(CommandContext ctx, [Description("Новый статус")] params string[] content)
            {
                var changedContent = string.Join(" ", content);

                if (changedContent == "reset")
                {
                    changedContent = "Zombies vs Plants";

                    var activity = new DiscordActivity
                    {
                        Name = $"{changedContent}",
                    };

                    var activityEmbed = new DiscordEmbedBuilder
                    {
                        Description = $"Статус бота был успешно восстановлен.",
                        Color = DiscordColor.Gold
                    };

                    await ctx.Client.UpdateStatusAsync(activity);
                    await ctx.Channel.SendMessageAsync(embed: activityEmbed).ConfigureAwait(false);
                }
                else
                {
                    var activityEmbed = new DiscordEmbedBuilder
                    {
                        Description = $"Статус бота успешно изменён на `{changedContent}`",
                        Color = DiscordColor.Gold
                    };

                    var activity = new DiscordActivity
                    {
                        Name = $"{changedContent}",
                    };

                    await ctx.Client.UpdateStatusAsync(activity);
                    await ctx.Channel.SendMessageAsync(embed: activityEmbed).ConfigureAwait(false);
                }
            }
        }
        
        [Command("roll")]
        [Description("Получение случайного числа")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Roll(CommandContext ctx, [Description("Минимальное число")] int min, [Description("Максимальное число")] int max)
        {
            var rnd = new Random();

            var rollEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold
            };

            if (max >= 1 && max > min)
            {
                rollEmbed.WithDescription($"🎲 Случайное число: {rnd.Next(min, max + 1)}");

                await ctx.Channel.SendMessageAsync(embed: rollEmbed).ConfigureAwait(false);
            }
            else if (max <= 1)
            {
                rollEmbed.WithDescription("Некорректно заданное число");

                await ctx.Channel.SendMessageAsync(embed: rollEmbed).ConfigureAwait(false);
            }
        }

        /*[Command("sugg")]
        [Description("Голосование за тему арта месяца для платных подписчиков")]
        [RequireRoles(RoleCheckMode.Any, "Twitch Sub", "Patreon Tier 3$")]
        public async Task Suggest(CommandContext ctx, [Description("Предложенная тема")] params string[] content)
        {
            var suggestEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            if (string.Join(" ", content) == string.Empty)
            {
                suggestEmbed.WithDescription("Вы не указали тему предложения");

                var joinMessage = await ctx.Channel.SendMessageAsync(embed: suggestEmbed).ConfigureAwait(false);
            }
            else
            {
                suggestEmbed.WithTitle("Предложение темы для арта месяца");
                suggestEmbed.WithDescription($"{ctx.User.Mention}: {string.Join(" ", content)}");

                var joinMessage = await ctx.Channel.SendMessageAsync(embed: suggestEmbed).ConfigureAwait(false);

                await joinMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":upVote:")).ConfigureAwait(false);
                await joinMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":downVote:")).ConfigureAwait(false);
            }
        }

        [Command("code")]
        [Hidden]
        [Description("Генерация случайного кода")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task WaitForCode(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var codeEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold,
            };

            var codebytes = new byte[8];
            using (var rnd = RandomNumberGenerator.Create())
                rnd.GetBytes(codebytes);

            var code = BitConverter.ToString(codebytes).ToLower().Replace("-", "");

            codeEmbed.WithDescription($"Первый, кто напишет код в чат, получит ничего: `{code}`");
            await ctx.Channel.SendMessageAsync(embed: codeEmbed).ConfigureAwait(false);

            var msg = await interactivity.WaitForMessageAsync(e => e.Content.Contains(code), TimeSpan.FromSeconds(60));

            codeEmbed.WithDescription($"Победитель: {msg.Result.Author.Mention}");
            await ctx.Channel.SendMessageAsync(embed: codeEmbed).ConfigureAwait(false);
        }*/

        [Command("botinfo")]
        [Description("Информация о боте")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Botinfo(CommandContext ctx)
        {
            var botInfoEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gold
            };
            botInfoEmbed.WithAuthor(ctx.Guild.CurrentMember.Username + "#" + ctx.Guild.CurrentMember.Discriminator, null, ctx.Guild.CurrentMember.AvatarUrl);
            botInfoEmbed.WithThumbnail("https://media.discordapp.net/attachments/720667905695678508/739437303499325510/cockflower-dance.gif", 500, 500);     
            botInfoEmbed.AddField("Исходный код:", "GitHub: https://github.com/aniv1re/SunflowerBot", false);
            botInfoEmbed.AddField("Автор аватара бота:", "VK: https://vk.com/pixel_youkai", false);
            botInfoEmbed.AddField("Пинг:", ctx.Client.Ping.ToString(), true);
            botInfoEmbed.AddField("ID бота:", ctx.Guild.CurrentMember.Id.ToString(), true);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(@"D:\code\Sunflower\Config.json"));
            botInfoEmbed.AddField("Версия:", configJson.Version, true);

            await ctx.Channel.SendMessageAsync(embed: botInfoEmbed).ConfigureAwait(false);
        }

        [Command("report")]
        [Description("Ссылка на страницу баг-репортов")]
        [RequireRoles(RoleCheckMode.None)]
        public async Task Report(CommandContext ctx)
        {
            var reportEmbed = new DiscordEmbedBuilder
            {
                Description = "Если вы нашли какие-то проблемы в работе бота или его команд, сообщите пожалуйста о них на странице GitHub",
                Color = DiscordColor.Gold
            };
            reportEmbed.WithTitle("Кликай сюда!");
            reportEmbed.WithUrl("https://github.com/aniv1re/SunflowerBot/issues/new");
            reportEmbed.AddField("GitHub:", "https://github.com/aniv1re/SunflowerBot/issues/");
            reportEmbed.WithThumbnail("https://avatars.mds.yandex.net/get-pdb/2303023/db0da5c7-0b20-4c7c-9014-425eab763af9/s1200?webp=false", 500, 500);

            await ctx.Channel.SendMessageAsync(embed: reportEmbed).ConfigureAwait(false);
        }
    }
}