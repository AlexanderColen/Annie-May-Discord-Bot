using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Reflection;
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

            _handler = new CommandHandler(_client, new CommandService());
            await _handler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Resources.DISCORD_BOT_TOKEN);
            await _client.StartAsync();

            // Start a neverending task that changes the status every X seconds.
            Task task = Task.Run(async () =>
            {
                Random rand = new Random();
                while (true)
                {
                    // Dirty way to get relative path.
                    string statusFilePath = Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\netcoreapp2.1\\AnnieMayDiscordBot.dll", "Resources\\Files\\statuses.txt");
                    // Load the statuses text file.
                    string[] lines = File.ReadAllLines(statusFilePath);
                    // Take a random status.
                    string status = lines[rand.Next(lines.Length)];
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
    }
}