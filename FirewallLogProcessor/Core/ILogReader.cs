namespace FirewallLogProcessor.Core;

public interface ILogReader
{
    IAsyncEnumerable<List<string>> ReadAsync();
}