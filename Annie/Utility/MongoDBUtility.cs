using Discord;

namespace AnnieMayDiscordBot.Utility
{
    public class MongoDBUtility
    {
        public IUser FindDiscordUserInChannel(IChannel channel, ulong userId)
        {
            return channel.GetUserAsync(userId).Result;
        }
    }
}
