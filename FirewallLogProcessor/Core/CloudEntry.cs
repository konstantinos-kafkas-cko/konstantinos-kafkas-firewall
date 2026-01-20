namespace FirewallLogProcessor.Core;

public class CloudEntry
{
    public string Name { get; init; } = null!;
    public string Domain { get; init; } = null!;
    public string Risk { get; init; } = null!;
    public string Country { get; init; } = null!;
    public bool GdprCompliant { get; init; }
}