namespace FirewallLogProcessor.Core;

public interface ILogProcessor
{
    IAsyncEnumerable<List<LogEntry>> ProcessAsync(IAsyncEnumerable<List<string>> chunks);
}