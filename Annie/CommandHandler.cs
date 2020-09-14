using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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
            var socketContext = new SocketCommandContext(_client, message);

            string prefix = Resources.PREFIX;
            int argPos = 0;

            // Get the settings that should be used for this Guild.
            CacheUtility.GetInstance().CachedGuildSettings.TryGetValue(socketContext.Guild.Id, out GuildSettings guildSettings);
            
            // If it was not found in the cached dictionary, look it up in the database.
            if (guildSettings == null)
            {
                guildSettings = await DatabaseUtility.GetInstance().GetSpecificGuildSettingsAsync(socketContext.Guild.Id);

                if (guildSettings != null)
                {
                    // Make sure to add guild settings to the dictionary to prevent future unnecessary database querying.
                    CacheUtility.GetInstance().CachedGuildSettings.Add(socketContext.Guild.Id, guildSettings);
                }
            }

            // Create the settings if it doesn't exist.
            if (guildSettings == null)
            {
                guildSettings = new GuildSettings
                {
                    GuildId = socketContext.Guild.Id,
                    Prefix = Resources.PREFIX,
                    ShowUserScores = true
                };

                // Make sure to add guild settings to the dictionary to prevent future unnecessary database querying.
                CacheUtility.GetInstance().CachedGuildSettings.Add(socketContext.Guild.Id, guildSettings);
            }
            else
            {
                prefix = guildSettings.Prefix;
            }

            // Check if the message starts with the indicated prefix, is not a mention of another user, and is not a bot message.
            if (!(message.HasStringPrefix(prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new CustomCommandContext(socketContext, guildSettings);

            // Notify users that command is being handled.
            await context.Channel.TriggerTypingAsync();

            // Handle the command by pointing to the appropriate module.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // Inform the user if the command fails.
            if (!result.IsSuccess)
            {
                await HandleError(result, context);
            }
        }

        /// <summary>
        /// Handle the error that was thrown by a command.
        /// </summary>
        /// <param name="result">The result after executing the command.</param>
        private async Task HandleError(IResult result, CustomCommandContext context)
        {
            // Command not found, react with question mark emoji.
            if (result.Error.Equals(CommandError.UnknownCommand))
            {
                try
                {
                    await context.Message.AddReactionAsync(new Emoji("\u2753"));
                }
                catch (HttpException)
                {
                    Console.WriteLine($"Reactions are not permitted in {context.Channel.Id}.");
                }
                return;
            }

            // Command found but parameter requirements not met, respond with asking for more parameters.
            if (result.Error.Equals(CommandError.BadArgCount))
            {
                try
                {
                    await context.Channel.SendMessageAsync($"`{context.Message.Content}` makes invalid use of parameters.");
                }
                catch (HttpException)
                {
                    Console.WriteLine($"Sending messages is not permitted in {context.Channel.Id}.");
                }
                return;
            }

            // Log the exception to a local file.
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Remove(0, 6).Remove(path.Length - 29);
            string fileName = $"{path}\\LOG_{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}-{DateTime.UtcNow.Day}.txt";
            using (var file = File.Exists(fileName) ? File.Open(fileName, FileMode.Append) : File.Open(fileName, FileMode.CreateNew))
            {
                using StreamWriter sw = new StreamWriter(file);
                sw.WriteLine($"[{DateTime.UtcNow}] {context.Message.Author.Username} sent: {context.Message.Content}");
                sw.WriteLine($"Error thrown: {result.ErrorReason}");
            }

            // Command threw an uncaught error, try to react with error emoji if reactions are allowed.
            try
            {
                await context.Message.AddReactionAsync(new Emoji("\uD83D\uDEAB"));
            }
            catch (HttpException)
            {
                Console.WriteLine($"Reactions are not permitted in {context.Channel.Id}.");
            }
        }
    }
}