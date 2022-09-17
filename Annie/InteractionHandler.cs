using AnnieMayDiscordBot.Properties;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace InteractionFramework
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration config)
        {
            _client = client;
            _handler = handler;
            _services = services;
            _configuration = config;
        }

        public async Task InitializeAsync()
        {
            // Process when the client is ready, so we can register our commands.
            _client.Ready += ReadyAsync;
            _handler.Log += LogAsync;

            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;
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

        private async Task ReadyAsync()
        {
            // For debug purposes, registering commands in a specific guild is faster.
            if (AnnieMayClient.IsDebug())
                await _handler.RegisterCommandsToGuildAsync(ulong.Parse(Resources.DEBUG_GUILD_ID), true);
            else
                await _handler.RegisterCommandsGloballyAsync(true);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_client, interaction);

                // Execute the incoming command.
                var result = await _handler.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            await interaction.RespondAsync(text: $"`{interaction.Data}` makes invalid use of parameters.", ephemeral: true);
                            break;

                        case InteractionCommandError.Exception:
                            await interaction.RespondAsync(text: "Sorry senpai, something went wrong. If this keeps occurring, you can contact my creator in this server: https://discord.gg/5qgNVxUNEA", ephemeral: true);
                            break;

                        default:
                            break;
                    }
            }
            catch
            {
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
