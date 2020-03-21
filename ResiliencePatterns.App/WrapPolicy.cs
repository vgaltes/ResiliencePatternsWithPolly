using Polly;

namespace ResiliencePatterns.App
{
    public class WrapPolicy
    {
        public static void Run()
        {
            var wrapPolicy = Policy.Wrap(
                    FallbackPolicy.GetPolicy(), 
                    RetryPolicy.GetPolicy<User>()
                );
        }
    }
}