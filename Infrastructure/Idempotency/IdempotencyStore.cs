using System.Collections.Concurrent;
using System.Text.Json;

namespace Infrastructure.Idempotency
{
    public class IdempotencyEntry
    {
        public int StatusCode { get; set; }
        public string ResponseJson { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }

    public class IdempotencyStore
    {
        private readonly ConcurrentDictionary<string, IdempotencyEntry> _store = new();
        private readonly TimeSpan _ttl;
 

        public IdempotencyStore(TimeSpan? ttl = null)
        {
            _ttl = ttl ?? TimeSpan.FromMinutes(10);
        }

        public bool TryGet(string key, out IdempotencyEntry entry)
        {
            CleanupIfExpired(key);
            return _store.TryGetValue(key, out entry);
        }

        public void Save(string key, int statusCode, object responseObject)
        {
            var entry = new IdempotencyEntry
            {
                StatusCode = statusCode,
                ResponseJson = JsonSerializer.Serialize(responseObject),
                CreatedAtUtc = DateTime.UtcNow
            };
            _store[key] = entry;
        }

        private void CleanupIfExpired(string key)
        {
            if (_store.TryGetValue(key, out var e))
            {
                if (DateTime.UtcNow - e.CreatedAtUtc > _ttl)
                {
                    _store.TryRemove(key, out _);
                }
            }
        }


    }
}
