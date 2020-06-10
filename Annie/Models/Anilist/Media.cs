using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Media
    {
        public int id { get; set; }
        public int idMal { get; set; }
        public MediaTitle title { get; set; }
        public MediaType type { get; set; }
        public MediaFormat format { get; set; }
        public MediaStatus status { get; set; }
        public string description { get; set; }
        public FuzzyDate startDate { get; set; }
        public FuzzyDate endDate { get; set; }
        public MediaSeason? season { get; set; }
        public int? seasonYear { get; set; }
        public int? seasonInt { get; set; }
        public int? episodes { get; set; }
        public int? duration { get; set; }
        public int? chapters { get; set; }
        public int? volumes { get; set; }
        public MediaSource source { get; set; }
        public int updatedAt { get; set; }
        public MediaCoverImage coverImage { get; set; }
        public string bannerImage { get; set; }
        public List<string> genres { get; set; }
        public List<string> synonyms { get; set; }
        public int averageScore { get; set; }
        public int? meanScore { get; set; }
        public int popularity { get; set; }
        public int favourites { get; set; }
        public MediaStats stats { get; set; }
        public string siteUrl { get; set; }
    }
}