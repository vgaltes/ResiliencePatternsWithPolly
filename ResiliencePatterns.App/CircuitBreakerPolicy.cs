using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;

namespace ResiliencePatterns.App
{
    public static class CircuitBreakerPolicy
    {
        public static async Task Run()
        {
            // Break the circuit after the specified number of consecutive exceptions
            // and keep circuit broken for the specified duration,
            // calling an action on change of circuit state.
            Action<Exception, TimeSpan> onBreak = (exception, timespan) => 
            {
                Console.WriteLine($"Breaking because of {exception.Message} after {timespan.TotalSeconds} seconds");
            };
            Action onReset = () => 
            { 
                Console.WriteLine("It's running again!");
            };
            
            var breakerPolicy = Policy
                .Handle<BreakerException>()
                .CircuitBreaker(1, TimeSpan.FromSeconds(10), onBreak, onReset);

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"Lets call the downstream service at {DateTime.Now.ToLongTimeString()}");

                try{
                    
                    breakerPolicy.Execute(ctx => SimulateCallToDownstreamService(ctx), 
                        new Dictionary<string, object>() {{"id", i}});
                } catch (Exception ex)
                {
                    Console.WriteLine($"The downstream service threw an exception: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        private static void SimulateCallToDownstreamService(Context ctx)
        {
            if (ctx.TryGetValue("id", out object id))
            {
                Console.WriteLine($"Calling downstream service for {id.ToString()} time at {DateTime.Now.ToLongTimeString()}");
                if (id.ToString() == "5") throw new BreakerException();
            }
        }
    }

    public class BreakerException : Exception 
    {
        public BreakerException() : base() {}
    }
}