using AnnieMayDiscordBot.Enums.GitHub;

namespace AnnieMayDiscordBot.Models.GitHub
{
    public class Issue
    {
        public Actor Author { get; set; }
        public int Number { get; set; }
        public IssueState State { get; set; }
        public string Title { get; set; }
    }
}