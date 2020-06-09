using AnnieMayDiscordBot.Properties;
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

            _handler = new CommandHandler(_client, new CommandService());
            await _handler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Resources.DISCORD_BOT_TOKEN);
            await _client.StartAsync();

            await _client.SetGameAsync($"{Resources.PREFIX}help", null, ActivityType.Listening);

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