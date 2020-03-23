using System;
using Polly;

namespace ResiliencePatterns.App
{
    public class WrapPolicy
    {
        public static void Run()
        {
            var wrapPolicy = Policy.Wrap(
                    FallbackPolicy.GetGenericPolicy(), 
                    RetryPolicy.GetPolicy<User>()
                );

            var executionCount = 0;
            var result = wrapPolicy.Execute(() =>
            {
                executionCount++;
                Console.WriteLine($"Executing action for {executionCount} time");
                throw new RetryException($"Trowing for execution {executionCount}");
                return new User("UserFromService");
            });

            Console.WriteLine($"Result is {result.Name}");
        }
    }
}