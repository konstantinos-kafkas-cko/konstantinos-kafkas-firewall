namespace FirewallLogProcessor.Core;

public interface IFilter
{
    public bool ShouldInclude(LogEntry record);
}