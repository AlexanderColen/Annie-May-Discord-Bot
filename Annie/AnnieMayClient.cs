using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnnieMayDiscordBot
{
    public class AnnieMayClient
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        private List<string> _statuses = new List<string>();

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
                FillStatusList();
                while (true)
                {
                    // Take a random status.
                    string status = _statuses[rand.Next(_statuses.Count)];
                    // Change the status.
                    await _client.SetGameAsync($"{Resources.PREFIX}help || {status}", null, ActivityType.Listening);
                    // Delay for 60000 milliseconds instead of Sleep to prevent from tying up the thread.
                    await Task.Delay(6000);
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

        /// <summary>
        /// Fill the _statuses list with predetermined statuses.
        /// </summary>
        private void FillStatusList()
        {
            // TODO: Move this to a database or local file.
            _statuses.Add("Thrashing MAL");
            _statuses.Add("Making fun of SAO");
            _statuses.Add("Rewatching Date a Live");
            _statuses.Add("大丈夫？");
            _statuses.Add("Anime was a mistake");
            _statuses.Add("死ね");
            _statuses.Add("Imagine all the fun we could have with my clones...");
            _statuses.Add("Notice me senpai~");
            _statuses.Add("ね~");
            _statuses.Add("Having a watch party with clones");
        }
    }
}