using AnnieMayDiscordBot.Enums.Anilist;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class UserStatusStatistic : AnilistSpecialUserStatistics
    {
        public MediaListStatus Status { get; set; }
    }
}