using InteractionFramework;

namespace Annie
{
    public class Program
    {
        public static void Main() => new AnnieMayClient().RunAsync()
                .GetAwaiter()
                .GetResult();
    }
}