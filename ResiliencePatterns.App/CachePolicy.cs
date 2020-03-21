using System;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;

namespace ResiliencePatterns.App
{
    public static class CachePolicy
    {
        public static void Run()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
            var policy =
                Policy
                    .Cache(
                        memoryCacheProvider,
                        new SlidingTtl(TimeSpan.FromMinutes(5)),
                        onCacheGet: (context, key) => 
                        {
                            Console.WriteLine($"Get {key} from cache");
                        },
                        onCachePut: (Context, key) => 
                        {
                            Console.WriteLine($"Put on {key}");
                        },
                        onCacheMiss: (context, key) => 
                        {
                            Console.WriteLine($"Miss {key}");
                        },
                        onCacheGetError: (context, key, exception) => 
                        {
                            Console.WriteLine($"Error getting {key}");
                        },
                        onCachePutError: (context, key, exception) => 
                        {
                            Console.WriteLine($"Error putting {key}");
                        });

            for (int i = 0; i < 10; i++)
            {
                var result = policy.ExecuteAndCapture(context => GetSomething(), new Context("KeyForSomething"));
                Console.WriteLine($"Result is {result.Result}");
            }
        }

        private static int GetSomething()
        {
            Console.WriteLine("Getting something for real");
            return 42;
        }
    }

    public class User
    {
        public string Name { get; }
        public User(string name)
        {
            Name = name;
        }
    }
}