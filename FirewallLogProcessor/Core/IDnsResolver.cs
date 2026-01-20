namespace FirewallLogProcessor.Core;

public interface IDnsResolver
{
    public Task<(bool Success, string? HostName)> ResolveAsync(string ipAddress, CancellationToken cancellationToken);
}