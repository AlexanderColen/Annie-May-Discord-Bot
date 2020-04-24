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

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            _handler = new CommandHandler(_client, new CommandService());
            await _handler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Properties.Resources.DISCORD_BOT_TOKEN);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
