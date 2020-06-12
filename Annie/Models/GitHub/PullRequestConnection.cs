using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.GitHub
{
    public class PullRequestConnection
    {
        public List<PullRequest> Nodes { get; set; }
        public int TotalCount { get; set; }
    }
}