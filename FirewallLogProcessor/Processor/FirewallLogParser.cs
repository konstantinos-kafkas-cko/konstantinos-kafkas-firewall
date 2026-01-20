using System.Globalization;
using System.Text.RegularExpressions;
using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.Processor;

public class FirewallLogParser : ILogParser
{
    private static readonly Regex KeyValueRegex =
        new(@"(\w+)=([^\s]+)", RegexOptions.Compiled);

    private static readonly string[] TimestampFormats =
    {
        "MMM  d HH:mm:ss",
        "MMM dd HH:mm:ss"
    };

    public bool TryParse(string line, out LogEntry record)
    {
        record = null!;

        if (string.IsNullOrWhiteSpace(line))
            return false;

        var matches = KeyValueRegex.Matches(line);
        if (matches.Count == 0)
            return false;

        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in matches)
            fields[m.Groups[1].Value] = m.Groups[2].Value;

        if (!fields.TryGetValue("SRC", out var src) ||
            !fields.TryGetValue("DST", out var dst))
            return false;

        if (!TryParseDirection(line, out var direction))
            return false;

        record = new LogEntry
        {
            Timestamp = ParseTimestamp(line),
            SourceIp = src,
            DestinationIp = dst,
            User = fields.GetValueOrDefault("USER"),
            Domain = fields.GetValueOrDefault("DOMAIN"),
            Direction = direction,
            Protocol = fields.GetValueOrDefault("PROTO") ?? "UNKNOWN"
        };

        return true;
    }

    private static bool TryParseDirection(string line, out TrafficDirection direction)
    {
        if (line.Contains("INBOUND", StringComparison.OrdinalIgnoreCase))
        {
            direction = TrafficDirection.Inbound;
            return true;
        }

        if (line.Contains("OUTG", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("OUTBOUND", StringComparison.OrdinalIgnoreCase))
        {
            direction = TrafficDirection.Outgoing;
            return true;
        }

        direction = default;
        return false;
    }

    private static DateTime ParseTimestamp(string line)
    {
        // Optional extra safety for odd lines; still minimal:
        if (line.Length < 15)
            return default;

        var ts = line[..15];
        return DateTime.ParseExact(ts, TimestampFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
    }
}
