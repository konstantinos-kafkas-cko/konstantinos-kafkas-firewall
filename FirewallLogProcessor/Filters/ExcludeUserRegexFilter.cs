using System.Text.RegularExpressions;
using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.Filters;

public class ExcludeUserRegexFilter : IFilter
{
    private readonly List<Regex> _patterns;

    public ExcludeUserRegexFilter(List<string> patterns)
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
        if (string.IsNullOrEmpty(record.User)) return true;

        return !_patterns.Any(r => r.IsMatch(record.User));
    }
}