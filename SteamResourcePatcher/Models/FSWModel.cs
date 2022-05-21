using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace SteamResourcePatcher.Models
{
    internal class FSWModel
    {
        private static FileSystemWatcher? s_watcher = null;
        private static readonly MemoryCache s_memCache = new(new MemoryCacheOptions());
        private static readonly TimeSpan s_cacheTimeSpan = TimeSpan.FromSeconds(1);

        internal static void CreateWatcher(string dir, PostEvictionDelegate postEvictionDelegate, IEnumerable<string>? filters = null)
        {
            s_watcher = new(dir)
            {
                IncludeSubdirectories = true
            };
            if (filters != null && filters.Any())
            {
                foreach (string? filter in filters)
                {
                    s_watcher.Filters.Add(filter);
                }
            }

            s_watcher.Changed += (s, e) => OnWatcherEvent(s, e, postEvictionDelegate);
            s_watcher.Created += (s, e) => OnWatcherEvent(s, e, postEvictionDelegate);
        }

        private static void OnWatcherEvent(object _, FileSystemEventArgs e, PostEvictionDelegate postEvictionDelegate)
        {
            if (!s_memCache.TryGetValue(e, out _))
            {
                MemoryCacheEntryOptions options = new()
                {
                    Priority = CacheItemPriority.NeverRemove
                };
                _ = options.AddExpirationToken(new CancellationChangeToken(new CancellationTokenSource(s_cacheTimeSpan).Token));
                _ = options.RegisterPostEvictionCallback(postEvictionDelegate);
                s_memCache.Set(e.Name, e, options);
            }
        }

        internal static void CreateAndStartWatcher(string dir, PostEvictionDelegate postEvictionDelegate, IEnumerable<string>? filters = null)
        {
            CreateWatcher(dir, postEvictionDelegate, filters);
            StartWatcher();
        }

        internal static void StartWatcher()
        {
            if (s_watcher != null)
            {
                s_watcher.EnableRaisingEvents = true;
                Console.WriteLine("Watcher started.");
            }
        }

        internal static void StopWatcher()
        {
            if (s_watcher != null)
            {
                s_watcher.EnableRaisingEvents = false;
                Console.WriteLine("Watcher stopped.");
            }
        }

        internal static void AddFilter(string filter)
        {
            if (s_watcher != null && !s_watcher.Filters.Contains(filter))
            {
                s_watcher.Filters.Add(filter);
            }
        }

        internal static void RemoveFilter(string filter)
        {
            if (s_watcher != null)
            {
                s_watcher.Filters.Remove(filter);
            }
        }
    }
}
