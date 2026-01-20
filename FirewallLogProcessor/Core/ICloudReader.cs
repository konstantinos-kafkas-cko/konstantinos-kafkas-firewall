namespace FirewallLogProcessor.Core;

public interface ICloudReader
{
    IEnumerable<CloudEntry> Read();
}