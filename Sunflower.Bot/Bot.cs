using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using Sunflower.Bot.Commands;
using Sunflower.DAL.Context;
using Sunflower.DAL.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sunflower.Bot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead(@"D:\code\Sunflower\Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
            };

            Client = new DiscordClient(config);

            Client.Ready += ClientReady;
            Client.MessageCreated += OnMessageCreated;
            Client.GuildMemberAdded += GuildMemberAdded;
            Client.GuildCreated += GuildCreate;
            //Client.GuildAvailable += GuildAvailable;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                DmHelp = false,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<SunflowerCommands>();
            Commands.RegisterCommands<MainCommands>();
            Commands.RegisterCommands<DataCommands>();

            Commands.CommandErrored += OnCommandError;

            Commands.SetHelpFormatter<DefaultHelpFormatter>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task OnCommandError(CommandErrorEventArgs e)
        {
            var errorEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold);

            if (e.Exception is ChecksFailedException)
            {
                var propError = (ChecksFailedException)e.Exception;

                if (propError.FailedChecks[0] is RequireRolesAttribute)
                {
                    errorEmbed.WithDescription("У вас нет необходимой роли на выполнение данной команды.");
                    await Client.SendMessageAsync(e.Context.Channel, embed: errorEmbed).ConfigureAwait(false);
                }
                else
                {
                    errorEmbed.WithDescription("У вас нет прав на выполнение данной команды.");
                    await Client.SendMessageAsync(e.Context.Channel, embed: errorEmbed).ConfigureAwait(false);
                }
            }           
        }

        private Task ClientReady(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", "Бот успешно загружен.", DateTime.Now);

            var activity = new DiscordActivity
            {
                Name = "Zombies vs Plants",
            };

            Client.UpdateStatusAsync(activity, UserStatus.Online, null);
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", "Статус бота успешно изменён.", DateTime.Now);

            return Task.CompletedTask;
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Автор: {e.Author.Username}, гильдия: {e.Guild.Name}, сообщение: {e.Message.Content}", DateTime.Now);
        }

        private static async Task GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
            {
                var check = false;

                if (usersContext.UserProfiles.Any(x => x.MemberId == e.Member.Id))
                {
                    check = true;
                }

                if (check == false)
                {
                    var user = new Profile()
                    {
                        GuildId = e.Guild.Id,
                        MemberId = e.Member.Id,
                        MemberUsername = e.Member.Username,
                        MemberSunCount = 0,
                        DailyCooldown = DateTime.Now
                    };

                    usersContext.UserProfiles.Add(user);
                    await usersContext.SaveChangesAsync();
                }
            }
        }

        private static async Task GuildCreate(GuildCreateEventArgs e)
        {
            using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
            {
                foreach (var item in e.Guild.Members)
                {
                    var check = false;

                    if (usersContext.UserProfiles.Any(x => (x.MemberId == item.Key) && x.GuildId == item.Value.Guild.Id))
                    {
                        check = true;
                    }

                    if (check == true)
                    {
                        continue;
                    }
                    else
                    {
                        var user = new Profile()
                        {
                            GuildId = e.Guild.Id,
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

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Бот присоединился к серверу {e.Guild.Name}, база данных была успешно обновлена.", DateTime.Now);
        }
    }
}
