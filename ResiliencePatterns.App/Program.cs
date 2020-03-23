using System.Threading.Tasks;

namespace ResiliencePatterns.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            switch(args[0])
            {
                case "retry": RetryPolicy.Run(); break;
                case "timeout": await TimeoutPolicy.Run(); break;
                case "cache": CachePolicy.Run(); break;
                case "fallback": FallbackPolicy.Run(); break;
                case "breaker": await CircuitBreakerPolicy.Run(); break;
                case "wrap": WrapPolicy.Run(); break;
            }
        }
    }
}
