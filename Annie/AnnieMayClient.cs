using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InteractionFramework
{
    public class AnnieMayClient
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        private readonly DiscordSocketConfig _socketConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        };

        public AnnieMayClient()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            _services = new ServiceCollection()
                .AddSingleton(_configuration)
                .AddSingleton(_socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }

        public async Task RunAsync()
        {
            var client = _services.GetRequiredService<DiscordSocketClient>();

            client.UserJoined += Greet;
            client.Log += LogAsync;

            // Here we can initialize the service that will register and execute our commands
            await _services.GetRequiredService<InteractionHandler>()
                .InitializeAsync();

            await client.LoginAsync(TokenType.Bot, Resources.DISCORD_BOT_TOKEN);
            await client.StartAsync();

            // Start a neverending task that changes the status every X seconds.
            Task task = Task.Run(async () =>
            {
                Random rand = new Random();
                var statuses = await DatabaseUtility.GetInstance().GetCustomStatuses();
                while (true)
                {
                    // Take a random status.
                    string status = statuses[rand.Next(statuses.Count)];
                    // Change the status.
                    await client.SetGameAsync($"Back in business with working Slash Commands! || /help || {status}", null, ActivityType.Listening);
                    // Delay for 60000 milliseconds instead of Sleep to prevent from tying up the thread.
                    await Task.Delay(60000);
                }
            });

            // Never quit the program until manually forced to.
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Write a message to the command line for logging purposes.
        /// </summary>
        /// <param name="msg">The message that needs to be logged.</param>
        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static bool IsDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }

        /// <summary>
        /// Greet a newly joined User and recommend them to setup their anilist in a specific server.
        /// </summary>
        /// <param name="guildUser">The User that joined.</param>
        private async Task Greet(SocketGuildUser guildUser)
        {
            // Don't greet bots.
            if (guildUser.IsBot)
            {
                return;
            }
            // 343060137164144642 Discord ID for Annak's Lair Guild
            // 716449418760552500 Discord ID for Annie May support server Guild
            if (guildUser.Guild.Id == 343060137164144642 || guildUser.Guild.Id == 716449418760552500)
            {
                try
                {
                    // Custom emoji for Annak's Lair.
                    var emoji = await guildUser.Guild.GetEmoteAsync(569163505186373642);
                    await guildUser.Guild.DefaultChannel.SendMessageAsync($"今日は {guildUser.Mention}! Welcome to {guildUser.Guild.Name}! {emoji}\n\n" +
                        $"Annie May is here to serve you 御主人様.\n\n" +
                        $"To be part of the fun, make sure to tell me your Anilist using `/setup anilist <USERNAME/ID>` in this server. (Or in private if you prefer that)\n\n" +
                        $"If you have any questions regarding my functionalities, the `/help` command may be of assistance.");
                }
                catch (Exception)
                {
                    await guildUser.Guild.DefaultChannel.SendMessageAsync($"今日は {guildUser.Mention}! Welcome to {guildUser.Guild.Name}! <Insert Greeting Emoji Here>\n\n" +
                        $"Annie May is here to serve you 御主人様.\n\n" +
                        $"To be part of the fun, make sure to tell me your Anilist using `/setup anilist <USERNAME/ID>` in this server. (Or in private if you prefer that)\n\n" +
                        $"If you have any questions regarding my functionalities, the `/help` command may be of assistance.");
                }
            }
        }
    }
}
