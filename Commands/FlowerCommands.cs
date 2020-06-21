using SunflowerBot.Attributes;
using DSharpPlus.CommandsNext;
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
        //[RequireCategories(ChannelCheckMode.Any, "Sunflower")]
        [RequireRoles(RoleCheckMode.All)]
        public async Task Sunny(CommandContext ctx)
        {
            var role = ctx.Guild.GetRole(720276705071333506);

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Хочешь получать уведомления о предстоящих солнечных эвентах?\n",
                Description = $":sunflower: Выбери эмодзи для получения роли.\n\nДанная роль позволяет разрешать упоминать вас ({role.Mention}), во время различных событий, для получения солнышек и других наград.\n\nПосле выбора эмодзи сообщение удалится и вы получите роль. Чтобы отписаться от рассылки наберите команду вновь и отреагируюте эмодзи.",
                Color = DiscordColor.Gold,
            };
            
            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);   

            var accept = DiscordEmoji.FromName(ctx.Client, ":sunflower:");

            await joinMessage.CreateReactionAsync(accept).ConfigureAwait(false);   

            var interactivity = ctx.Client.GetInteractivity();

            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == joinMessage && 
                x.User == ctx.User &&
                (x.Emoji == accept)).ConfigureAwait(false);

            if (reactionResult.Result.Emoji == accept) 
            {
                await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
            }

            await joinMessage.DeleteAsync().ConfigureAwait(false);
        }    
        /*
        [Command("giveaway")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task Giveaway(CommandContext ctx, TimeSpan duration, int sunCount)
        {
            Random rnd = new Random();
            var interactivity = ctx.Client.GetInteractivity();

            var giveawayEmbed = new DiscordEmbedBuilder
            {
                Title = "Раздача солнышек",
                Description = $"В случае вашей победы Вы получите **{sunCount}** солнышек!",
                Color = DiscordColor.Gold
            };
            giveawayEmbed.WithFooter($"Оставшееся время - {duration}", null);

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: giveawayEmbed).ConfigureAwait(false);
        */

        [Command("info")]
        [Description("Информация и ссылки")]
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

        
        [Command("patreon")]
        [Description("Платный контент")]
        [RequireRoles(RoleCheckMode.All)]
        public async Task Patreon(CommandContext ctx)
        {
            var patreonEmbed = new DiscordEmbedBuilder
            {
                Title = "Уникальный контент для платных подписчиков!",
                Description = $"У вас есть возможно подписаться за 1$ или 3$ на Patreon, чтобы получить доступ к уникальному контенту, секретным эвентам и преимуществам в Discord.\n\nPatreon: https://www.patreon.com/aniv1re",
                Color = DiscordColor.Gold,
            };
            patreonEmbed.WithThumbnail("https://cahoicatam.com/images/patreon-logo-png-white-1.png", 1000, 500);
            patreonEmbed.WithFooter("Все преимущества от тиров Patreon'а даёт также платная подписка на Twitch");

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: patreonEmbed).ConfigureAwait(false);   
        }   
    }
}