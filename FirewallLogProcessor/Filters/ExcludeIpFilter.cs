using System.Formats.Asn1;
using FirewallLogProcessor.Core;
using FirewallLogProcessor.Utils;

namespace FirewallLogProcessor.Filters;

public sealed class ExcludeIpFilter : IFilter
{
    private readonly List<string> _rules;

    public ExcludeIpFilter(List<string> rules)
    {
        if (rules is null)
            throw new ArgumentNullException(nameof(rules));

        if (!rules.Any())
            throw new ArgumentException("The rules collection must contain at least one entry.", nameof(rules));

        _rules = rules;
    }

    public bool ShouldInclude(LogEntry record)
    {
        if (record.InternalIp == null) return false;

        return !_rules.Any(rule => IpMatcher.Matches(record.InternalIp, rule));
    }
}