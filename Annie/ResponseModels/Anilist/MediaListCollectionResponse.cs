using AnnieMayDiscordBot.Models.Anilist;

namespace AnnieMayDiscordBot.ResponseModels.Anilist
{
    public class MediaListCollectionResponse
    {
        public MediaListCollection MediaListCollection { get; set; }
        public MediaListCollection AnimeList { get; set; }
        public MediaListCollection MangaList { get; set; }
    }
}