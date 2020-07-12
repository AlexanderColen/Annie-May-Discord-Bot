using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot
{
    public class AnnieMayClient
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        /// <summary>
        /// Client starter method that starts all asynchronous functions and puts the bot online without halting afterwards.
        /// </summary>
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.UserJoined += Greet;

            _handler = new CommandHandler(_client, new CommandService());
            await _handler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Resources.DISCORD_BOT_TOKEN);
            await _client.StartAsync();

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
                    await _client.SetGameAsync($"{Resources.PREFIX}help || {status}", null, ActivityType.Listening);
                    // Delay for 60000 milliseconds instead of Sleep to prevent from tying up the thread.
                    await Task.Delay(60000);
                }
            });

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        /// <summary>
        /// Write a message to the command line for logging purposes.
        /// </summary>
        /// <param name="msg">The message that needs to be logged.</param>
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
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
            if (guildUser.Guild.Id == 716449418760552500 || guildUser.Guild.Id == 716449418760552500)
            {
                try
                {
                    // Custom emoji for Annak's Lair.
                    var emoji = await guildUser.Guild.GetEmoteAsync(569163505186373642);
                    await guildUser.Guild.DefaultChannel.SendMessageAsync($"今日は {guildUser.Mention}! Welcome to {guildUser.Guild.Name}! {emoji}\n\n" +
                        $"Annie May is here to serve you 御主人様.\n\n" +
                        $"To be part of the fun, make sure to tell me your Anilist using `{Resources.PREFIX}setup anilist <USERNAME/ID>` either in this server or in private if you prefer that.\n\n" +
                        $"If you have any questions regarding my functionalities, the `{Resources.PREFIX}help` may be of assistance.");
                }
                catch (Exception)
                {
                    await guildUser.Guild.DefaultChannel.SendMessageAsync($"今日は {guildUser.Mention}! Welcome to {guildUser.Guild.Name}! <Insert Greeting Emoji Here>\n\n" +
                        $"Annie May is here to serve you 御主人様.\n\n" +
                        $"To be part of the fun, make sure to tell me your Anilist using `{Resources.PREFIX}setup anilist <USERNAME/ID>` either in this server or in private if you prefer that.\n\n" +
                        $"If you have any questions regarding my functionalities, the `{Resources.PREFIX}help` may be of assistance.");
                }
            }
        }
    }
}