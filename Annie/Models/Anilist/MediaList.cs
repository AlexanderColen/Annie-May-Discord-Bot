using AnnieMayDiscordBot.Enums.Anilist;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class MediaList
    {
        public int id { get; set; }
        public int mediaId { get; set; }
        public MediaListStatus? status { get; set; }
        public float score { get; set; }
        public int progress { get; set; }
        public int progressVolumes { get; set; }
        public int repeat { get; set; }
        public int priority { get; set; }
        public FuzzyDate startedAt { get; set; }
        public FuzzyDate completedAt { get; set; }
        public int updatedAt { get; set; }
        public int createdAt { get; set; }
        public Media media { get; set; }
    }
}
