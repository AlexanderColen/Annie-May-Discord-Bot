using AnnieMayDiscordBot;
using System.Threading.Tasks;

namespace Annie
{
    public class Program
    {
        public static void Main(string[] args) => new AnnieMayClient().MainAsync().Wait();
    }
}
