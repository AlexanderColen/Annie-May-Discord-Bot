using System;
using System.Reflection;
using System.Threading.Tasks;
using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AnnieMayDiscordBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message))
            {
                return;
            }
            
            int argPos = 0;
            
            if (!(message.HasStringPrefix(Resources.PREFIX, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;
            
            Console.WriteLine($"\n{message.Author.Username} sent: {message.Content}\n");
            var context = new SocketCommandContext(_client, message);

            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // Inform the user if the command fails.
            if (!result.IsSuccess)
            {
                Console.WriteLine($"{result.ErrorReason}");

                if (result.Error.Equals(CommandError.UnknownCommand))
                {
                    await context.Message.AddReactionAsync(new Emoji("\u2753"));
                    return;
                }

                if (result.Error.Equals(CommandError.BadArgCount))
                {
                    await context.Channel.SendMessageAsync($"`{context.Message.Content}` requires more parameters.");
                    return;
                }

                await context.Message.AddReactionAsync(new Emoji("\uD83D\uDEAB"));
            }
        }
    }
}
