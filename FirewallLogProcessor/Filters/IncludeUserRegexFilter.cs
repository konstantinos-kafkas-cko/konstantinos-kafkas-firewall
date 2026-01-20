using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.Filters;

using System.Text.RegularExpressions;

public sealed class IncludeUserRegexFilter : IFilter
{
    private readonly List<Regex> _patterns;

    public IncludeUserRegexFilter(List<string> patterns)
    {
        if (patterns is null)
            throw new ArgumentNullException(nameof(patterns));

        if (!patterns.Any())
            throw new ArgumentException("The collection must contain at least one regex pattern.", nameof(patterns));
        
        _patterns = patterns
            .Select(p => new Regex(p, RegexOptions.Compiled | RegexOptions.CultureInvariant))
            .ToList();
    }

    public bool ShouldInclude(LogEntry record)
    {
        if (string.IsNullOrEmpty(record.User)) return false;

        return _patterns.Any(r => r.IsMatch(record.User));
    }
}
