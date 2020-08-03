using Sunflower.Bot.Commands;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sunflower.Context;
using Sunflower.Models;

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

            using (var fs = File.OpenRead(@"D:\code\Sunflower\Sunflower\Config.json"))
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

            Commands.SetHelpFormatter<DefaultHelpFormatter>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }
        
        private Task ClientReady(ReadyEventArgs ctx)
        {
            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", "Бот успешно загружен.", DateTime.Now);

            var activity = new DiscordActivity
            {
                Name = "Zombies vs Plants",
            };

            Client.UpdateStatusAsync(activity, UserStatus.Online, null);

            return Task.CompletedTask;
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs ctx)
        {
            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Автор: {ctx.Author.Username}, гильдия: {ctx.Guild.Name}, сообщение: {ctx.Message.Content}", DateTime.Now);
        }

        private static async Task GuildMemberAdded(GuildMemberAddEventArgs ctx)
        {
            var user = new Profile();
            user.MemberId = ctx.Member.Id;
            user.MemberUsername = ctx.Member.Username;
            user.MemberSunCount = 0;
            user.DailyCooldown = DateTime.Now.Date;

            using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
            {
                usersContext.Users.Add(user);
                await usersContext.SaveChangesAsync();
            }
        }

        private static async Task GuildCreate(GuildCreateEventArgs ctx)
        {
            var users = ctx.Guild.Members;

            foreach (var item in users)
            {
                var user = new Profile();
                user.MemberId = item.Key;
                user.MemberUsername = item.Value.Username;
                user.MemberSunCount = 0;
                user.DailyCooldown = DateTime.Now.Date;

                using (SunflowerUsersContext usersContext = new SunflowerUsersContext())
                {
                    usersContext.Users.Add(user);
                    await usersContext.SaveChangesAsync();
                }
            }

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, "Sunflower", $"Бот присоединился к серверу {ctx.Guild.Name}, база данных была успешно обновлена.", DateTime.Now);
        }
    }
}
