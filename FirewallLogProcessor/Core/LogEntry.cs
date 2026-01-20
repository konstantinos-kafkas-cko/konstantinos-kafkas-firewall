namespace FirewallLogProcessor.Core;

public class LogEntry
{
    public DateTime Timestamp { get; init; }

    public string SourceIp { get; init; } = null!;
    public string DestinationIp { get; init; } = null!;

    public string? User { get; init; }
    public string? Domain { get; set; }

    public TrafficDirection Direction { get; init; } // INBOUND // OUTG
    public string Protocol { get; init; } = null!;
    
    public string InternalIp =>
        Direction == TrafficDirection.Outgoing
            ? SourceIp
            : DestinationIp;
    
    public string ExternalIp =>
        Direction == TrafficDirection.Outgoing ? 
            DestinationIp 
            : SourceIp;
}