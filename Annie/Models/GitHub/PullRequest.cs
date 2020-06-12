using AnnieMayDiscordBot.Enums.GitHub;

namespace AnnieMayDiscordBot.Models.GitHub
{
    public class PullRequest
    {
        public Actor Author { get; set; }
        public int Number { get; set; }
        public PullRequestState State { get; set; }
        public string Title { get; set; }
    }
}