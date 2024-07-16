using System.Collections.Concurrent;
using GHotel.Application.Utilities;

namespace GHotel.Infrastructure.Utilities;

public class SemaphorLockUtility : ILockUtility
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _roomLocks = new(StringComparer.Ordinal);

    public async Task Lock(string key)
    {
        var semaphore = _roomLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync().ConfigureAwait(false);
    }

    public void Release(string key)
    {
        if (_roomLocks.TryRemove(key, out var semaphore))
            semaphore.Release();
    }
}
