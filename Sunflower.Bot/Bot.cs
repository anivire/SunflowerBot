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
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

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
            Client.MessageReactionAdded += ReactionAdded;
            Client.MessageReactionRemoved += ReactionRemoved;
            Client.GuildAvailable += GuildAvaible;

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
                EnableDefaultHelp = true,
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

        public async Task Timer()
        {
            Timer t = new Timer(300000);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            t.Start();

            Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Таймер проверки сервером успешно запущен.", DateTime.Now);
        }
        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var serversEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Gold
            };

            var listName = String.Empty;
            var listID = String.Empty;
            var listMembers = String.Empty;

            foreach (var item in Client.Guilds.Values)
            {
                listName += string.Join(" ", item.Name + "\n");
                listID += string.Join(" ", item.Id + "\n");
                listMembers += string.Join(" ", item.MemberCount + "\n");
            }

            serversEmbed.WithAuthor("Список серверов, на которых есть бот:", null, Client.Guilds.Values.Single(x => x.Id == 720667905695678503).CurrentMember.AvatarUrl);
            serversEmbed.AddField("Название:", listName, true);
            serversEmbed.AddField("ID сервера:", listID, true);
            serversEmbed.AddField("Участники:", listMembers, true);
            serversEmbed.WithFooter("Список серверов обновится через 5 минут • " + DateTime.Now);

            DiscordEmbed newEmbed = serversEmbed;

            await Client.Guilds.Values.Single(x => x.Id == 720667905695678503).GetChannel(741634733745897512).GetMessageAsync(741642171488403496).Result.ModifyAsync(embed: newEmbed);
        }

        private async Task GuildAvaible(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Проверка на целостность базы данных для гильдии {e.Guild.Name} начата...", DateTime.Now);

            using DatabaseContext databaseContext = new DatabaseContext();

            foreach (var item in e.Guild.Members)
            {
                if (databaseContext.UserProfiles.Any(x => (x.MemberId == item.Key) && x.GuildId == item.Value.Guild.Id) == true)
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

                    databaseContext.UserProfiles.Add(user);
                    await databaseContext.SaveChangesAsync();
                }
            }

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Проверка баз данных для гильдии {e.Guild.Name} была успешно завершена.", DateTime.Now);
        }

        private async Task ReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            using DatabaseContext databaseContext = new DatabaseContext();
            if (databaseContext.SunnyMessage.Any(x => x.MessageId == e.Message.Id))
            {
                if (e.Emoji.Name == DiscordEmoji.FromName(e.Client, ":sunflower:"))
                {
                    foreach (var item in databaseContext.SunnyMessage)
                    {
                        await e.Guild.Members.Values.Single(x => x.Id == e.User.Id).RevokeRoleAsync(e.Guild.Roles.Values.Single(x => x.Id == item.RoleId));
                    }
                }
            }
        }

        private async Task ReactionAdded(MessageReactionAddEventArgs e)
        {
            using DatabaseContext databaseContext = new DatabaseContext();
            if (databaseContext.SunnyMessage.Any(x => x.MessageId == e.Message.Id))
            {
                if (e.Emoji.Name == DiscordEmoji.FromName(e.Client, ":sunflower:"))
                {
                    foreach (var item in databaseContext.SunnyMessage)
                    {
                        await e.Guild.Members.Values.Single(x => x.Id == e.User.Id).GrantRoleAsync(e.Guild.Roles.Values.Single(x => x.Id == item.RoleId));
                    }
                }
            }
        }

        private async Task OnCommandError(CommandErrorEventArgs e)
        {
            var errorEmbed = new DiscordEmbedBuilder().WithColor(DiscordColor.Gold);

            if (e.Exception is ChecksFailedException)
            {
                ChecksFailedException propError = (ChecksFailedException)e.Exception;

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
            else
            {
                Console.WriteLine(e.Exception.Message);
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

            Timer();

            return Task.CompletedTask;
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Автор: {e.Author.Username}, гильдия: {e.Guild.Name}, сообщение: {e.Message.Content}", DateTime.Now);
        }

        private static async Task GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            using DatabaseContext databaseContext = new DatabaseContext();

            if (databaseContext.UserProfiles.Any(x => (x.MemberId == e.Member.Id) && x.GuildId == e.Guild.Id) == false)
            {
                var user = new Profile()
                {
                    GuildId = e.Guild.Id,
                    MemberId = e.Member.Id,
                    MemberUsername = e.Member.Username,
                    MemberSunCount = 0,
                    DailyCooldown = DateTime.Now
                };

                databaseContext.UserProfiles.Add(user);
                await databaseContext.SaveChangesAsync();
            }
        }

        private static async Task GuildCreate(GuildCreateEventArgs e)
        {
            using DatabaseContext databaseContext = new DatabaseContext();

            foreach (var item in e.Guild.Members)
            {
                if (databaseContext.UserProfiles.Any(x => (x.MemberId == item.Key) && x.GuildId == item.Value.Guild.Id) == true)
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

                    databaseContext.UserProfiles.Add(user);
                    await databaseContext.SaveChangesAsync();
                }

            }
            

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Бот присоединился к серверу {e.Guild.Name}, база данных была успешно обновлена.", DateTime.Now);
        }
    }
}
