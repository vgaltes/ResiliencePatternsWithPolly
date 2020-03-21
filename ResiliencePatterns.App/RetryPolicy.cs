using System;
using Polly;
using Polly.Retry;

namespace ResiliencePatterns.App
{
    public static class RetryPolicy
    {
        public static void Run()
        {
            Polly.Retry.RetryPolicy a;
            RetryPolicy<int> policy = GetPolicy<int>();

            var executionCount = 0;
            var result = policy.Execute(() =>
            {
                executionCount++;
                Console.WriteLine($"Executing action for {executionCount} time");
                throw new RetryException($"Trowing for execution {executionCount}");
                return 2;
            });

            Console.WriteLine($"Result is {result}");
        }

        public static Polly.Retry.RetryPolicy AnotherPolicy(){
            return Policy.Handle<RetryException>().WaitAndRetry(1, (i,t) =>  TimeSpan.FromSeconds(2));
        }
        public static RetryPolicy<A> GetPolicy<A>()
        {
            return Policy<A>
                    .Handle<RetryException>()
                    .WaitAndRetry(
                        3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (result, timeSpan, retryCount, context) =>
                        {
                            Console.WriteLine($"Retrying for {retryCount} time, on time {timeSpan} for exception {result.Exception.Message}");
                        }
                    );
        }

    }

    public class RetryException : Exception 
    {
        public RetryException(string message) : base(message) {}
    }
}