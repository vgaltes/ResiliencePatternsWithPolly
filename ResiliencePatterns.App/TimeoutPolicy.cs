using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace ResiliencePatterns.App
{
    public static class TimeoutPolicy
    {
        public static async Task Run()
        {
            AsyncTimeoutPolicy policy = GetPolicy();

            var cancellationToken = new CancellationToken();
            await policy.ExecuteAsync(async (ct) =>
            {
                Console.WriteLine($"Start Execution at {DateTime.UtcNow.ToLongTimeString()}");
                await Task.Delay(10000, ct);
                Console.WriteLine($"Finish Execution at {DateTime.UtcNow.ToLongTimeString()}");
            }, cancellationToken);

            Console.WriteLine("Done");
        }

        public static AsyncTimeoutPolicy GetPolicy()
        {
            var policy =
                Policy
                    .TimeoutAsync(
                        TimeSpan.FromSeconds(2),
                        // TimeoutStrategy.Pessimistic, // Optimistic, the default value, is for tasks that accepts a cancellation token
                        (context, timeSpan, task) =>
                        {
                            // Option 1: throws a Polly.Timeout.TimeoutRejectedException
                            Console.WriteLine($"Executiont timed out after {timeSpan.TotalSeconds} seconds at {DateTime.UtcNow.ToLongTimeString()}.");
                            return Task.CompletedTask;

                            // Option 2
                            return task.ContinueWith(t =>
                            {
                                Console.WriteLine($"Break Execution at {DateTime.UtcNow.ToLongTimeString()}");
                                if (t.IsFaulted)
                                {
                                    Console.WriteLine($"Execution timed out after {timeSpan.TotalSeconds} seconds. Faulted with {t.Exception}");
                                }
                                else if (t.IsCanceled)
                                {
                                    Console.WriteLine($"Execution cancelled after {timeSpan.TotalSeconds} seconds.");
                                }
                                else if (t.IsCompleted)
                                {
                                    Console.WriteLine($"Execution completed after {timeSpan.TotalSeconds} seconds. Faulted with {t.Exception}");
                                }
                            });
                        });
            return policy;
        }
    }
}