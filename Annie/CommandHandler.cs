using AnnieMayDiscordBot.Models;
using AnnieMayDiscordBot.Properties;
using AnnieMayDiscordBot.Utility;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
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

            // Handle the command by pointing to the appropriate module.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // Inform the user if the command fails.
            if (!result.IsSuccess)
            {
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
                
                Console.WriteLine($"{message.Author.Username} sent: {message.Content}\n");
                Console.WriteLine($"{result.ErrorReason}");

                // Command threw an uncaught error, react with error emoji.
                await context.Message.AddReactionAsync(new Emoji("\uD83D\uDEAB"));
            }
        }
    }
}