using System;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.GitHub
{
    public class Repository
    {
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public List<IssueConnection> Issues { get; set; }
        public string Name { get; set; }
        public Language PrimaryLanguage { get; set; }
        public List<PullRequestConnection> PullRequests { get; set; }
        public DateTime PushedAt { get; set; }
        public Uri Url { get; set; }
    }
}