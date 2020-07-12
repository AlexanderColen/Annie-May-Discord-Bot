using Discord;
using Discord.Commands;

namespace AnnieMayDiscordBot.Models
{
    public class CustomCommandContext : ICommandContext
    {
        public IDiscordClient Client { get; }

        public IGuild Guild { get; }

        public IMessageChannel Channel { get; }

        public IUser User { get; }

        public IUserMessage Message { get; }

        public SocketCommandContext SocketCommandContext { get; }

        public GuildSettings Settings { get; }

        public CustomCommandContext(SocketCommandContext socketCommandContext, GuildSettings guildSettings)
        {
            SocketCommandContext = socketCommandContext;
            Settings = guildSettings;
            Client = socketCommandContext.Client;
            Guild = socketCommandContext.Guild;
            Channel = socketCommandContext.Channel;
            User = socketCommandContext.User;
            Message = socketCommandContext.Message;
        }
    }
}