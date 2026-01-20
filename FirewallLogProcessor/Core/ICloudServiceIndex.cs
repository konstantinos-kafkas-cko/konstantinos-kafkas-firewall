namespace FirewallLogProcessor.Core;

public interface ICloudServiceIndex
{
    CloudEntry? FindByDomain(string domain);
}