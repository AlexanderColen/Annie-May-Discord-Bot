using AnnieMayDiscordBot.Enums.Anilist;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.Anilist
{
    public class Media
    {
        public int Id { get; set; }
        public int? IdMal { get; set; }
        public MediaTitle Title { get; set; }
        public MediaType Type { get; set; }
        public MediaFormat? Format { get; set; }
        public MediaStatus? Status { get; set; }
        public string Description { get; set; }
        public FuzzyDate StartDate { get; set; }
        public FuzzyDate EndDate { get; set; }
        public MediaSeason? Season { get; set; }
        public int? SeasonYear { get; set; }
        public int? SeasonInt { get; set; }
        public int? Episodes { get; set; }
        public int? Duration { get; set; }
        public int? Chapters { get; set; }
        public int? Volumes { get; set; }
        public MediaSource Source { get; set; }
        public int UpdatedAt { get; set; }
        public MediaCoverImage CoverImage { get; set; }
        public string BannerImage { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Synonyms { get; set; }
        public int AverageScore { get; set; }
        public int? MeanScore { get; set; }
        public int Popularity { get; set; }
        public int Favourites { get; set; }
        public MediaStats Stats { get; set; }
        public string SiteUrl { get; set; }
    }
}