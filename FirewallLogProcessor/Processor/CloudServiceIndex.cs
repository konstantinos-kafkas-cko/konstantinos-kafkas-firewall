using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.Processor;

public sealed class CloudServiceIndex : ICloudServiceIndex
{
    private readonly Dictionary<string, CloudEntry> _byDomain;

    public CloudServiceIndex(IEnumerable<CloudEntry> services)
    {
        _byDomain = services.ToDictionary(
            s => NormalizeDomain(s.Domain),
            s => s,
            StringComparer.OrdinalIgnoreCase);
    }

    public CloudEntry? FindByDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            return null;

        _byDomain.TryGetValue(NormalizeDomain(domain), out var service);
        return service;
    }

    private static string NormalizeDomain(string domain)
    {
        domain = domain.Trim().ToLowerInvariant();

        if (domain.StartsWith("www."))
            domain = domain[4..];

        return domain;
    }
}