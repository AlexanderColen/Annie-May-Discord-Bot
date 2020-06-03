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

        /// <summary>
        /// Constructor that initializes the CommandService and DiscordSocketClient.
        /// </summary>
        /// <param name="client">The DiscordSocketClient.</param>
        /// <param name="commands">The CommandService.</param>
        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        /// <summary>
        /// Map message received event to the handling of commands and autoload all the Command Modules.
        /// </summary>
        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        /// <summary>
        /// Handles messages as commands when conditions are met and sends appropriate responses.
        /// </summary>
        /// <param name="messageParam">The Discord message that was sent by a user.</param>
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Make sure that the message is sent by a Discord User.
            if (!(messageParam is SocketUserMessage message))
            {
                return;
            }
            
            int argPos = 0;
            
            // Check if the message starts with the indicated prefix, is not a mention of another user and is not a bot message.
            if (!(message.HasStringPrefix(Resources.PREFIX, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;
            
            Console.WriteLine($"\n{message.Author.Username} sent: {message.Content}\n");
            var context = new SocketCommandContext(_client, message);

            // Handle the command by pointing to the appropriate module.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // Inform the user if the command fails.
            if (!result.IsSuccess)
            {
                Console.WriteLine($"{result.ErrorReason}");

                // Command not found, react with question mark emoji.
                if (result.Error.Equals(CommandError.UnknownCommand))
                {
                    await context.Message.AddReactionAsync(new Emoji("\u2753"));
                    return;
                }

                // Command found but parameter requirements not met, respond with asking for more parameters.
                if (result.Error.Equals(CommandError.BadArgCount))
                {
                    await context.Channel.SendMessageAsync($"`{context.Message.Content}` requires more parameters.");
                    return;
                }

                // Command threw an uncaught error, react with error emoji.
                await context.Message.AddReactionAsync(new Emoji("\uD83D\uDEAB"));
            }
        }
    }
}
