using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunflowerBot.Commands
{
    
    public class TestCommands : BaseCommandModule
    {
        [Command("respondmessage")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task RespondMessage(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
            
            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        [Command("respondreaction")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task RespondReaction(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel && x.User == ctx.User).ConfigureAwait(false);
            
            await ctx.Channel.SendMessageAsync(message.Result.Emoji);
        }

        [Command("pool")]
        [RequireRoles(RoleCheckMode.All, "Sun Sponsor")]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var results = distinctResult.Select(x => $"{x.Emoji}: {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }
    }
}