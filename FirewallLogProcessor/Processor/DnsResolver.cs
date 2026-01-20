using System.Net;
using System.Net.Sockets;
using FirewallLogProcessor.Core;
using Microsoft.Extensions.Caching.Memory;

namespace FirewallLogProcessor.Processor;

public class DnsResolver : IDnsResolver
{
    private readonly IMemoryCache _memoryCache;

    public DnsResolver(int memorySizeLimit)
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = memorySizeLimit
        });
    }

    public async Task<(bool Success, string? HostName)> ResolveAsync(
        string ipAddress,
        CancellationToken cancellationToken)
    {
        ipAddress = ipAddress.Trim();

        return await _memoryCache.GetOrCreateAsync(
            ipAddress,
            async entry =>
            {
                entry.Size = 1;

                try
                {
                    var hostEntry = await Dns
                        .GetHostEntryAsync(ipAddress, cancellationToken)
                        .ConfigureAwait(false);

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return (true, hostEntry.HostName);
                }
                catch (SocketException)
                {
                    // negative cache
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                    return (false, null);
                }
            });
    }
}