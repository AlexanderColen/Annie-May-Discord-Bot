using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnnieMayDiscordBot.Models.GitHub
{
    public class IssueConnection
    {
        public List<Issue> Nodes { get; set; }
        public int TotalCount { get; set; }
    }
}