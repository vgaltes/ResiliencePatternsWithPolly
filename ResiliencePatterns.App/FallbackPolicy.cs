using System;
using Polly;

namespace ResiliencePatterns.App
{
    public static class FallbackPolicy
    {
        public static void Run()
        {
            var policy = GetPolicy();

            var result = policy.Execute(() => throw new FallbackException());

            Console.WriteLine($"Username is {result.Name}");
        }

        public static Polly.Fallback.FallbackPolicy<User> GetPolicy()
        {
            return Policy<User>
                    .Handle<FallbackException>()
                    .Fallback<User>(() => new User("defaultUser"));
        }

        public static Polly.Fallback.FallbackPolicy<User> GetGenericPolicy()
        {
            return Policy<User>
                    .Handle<RetryException>()
                    .Fallback<User>(() => new User("defaultUser"));
        }
    }   

    public class FallbackException : Exception 
    {
        public FallbackException() : base() {}
    }
}