namespace FirewallLogProcessor.Core;

public interface ILogParser
{
    public bool TryParse(string line, out LogEntry logEntry);
}