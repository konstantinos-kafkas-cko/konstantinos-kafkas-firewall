using System.Collections.Concurrent;
using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.Processor;

public class LogProcessor
{
    private readonly ILogParser _parser;
    private readonly IEnumerable<IFilter> _filters;
    private readonly IDnsResolver _dnsResolver;
    private readonly ICloudServiceIndex _serviceIndex;
    
    public LogProcessor(
        ILogParser parser,
        IEnumerable<IFilter> filters,
        IDnsResolver dnsResolver,
        ICloudServiceIndex serviceIndex)
    {
        _parser = parser;
        _filters = filters;
        _dnsResolver = dnsResolver;
        _serviceIndex = serviceIndex;
    }
    
    public async Task<List<LogOutput>> ProcessAsync(List<string> chunks, CancellationToken cancellationToken)
    {
        var results = new ConcurrentBag<LogOutput>();

        var options = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 8
        };
        await Parallel.ForEachAsync(chunks, options, async (line, ct) =>
        {
            if (!_parser.TryParse(line, out LogEntry record))
                return;

            if (!_filters.All(f => f.ShouldInclude(record)))
                return;

            if (string.IsNullOrWhiteSpace(record.Domain))
            {
                var externalIp = record.ExternalIp;
                if (!string.IsNullOrWhiteSpace(externalIp))
                {
                    var dnsResult = await _dnsResolver.ResolveAsync(externalIp, ct);
                    if (dnsResult.Success)
                        record.Domain = dnsResult.HostName;
                }
            }

            if (string.IsNullOrWhiteSpace(record.Domain))
                return;

            var service = _serviceIndex.FindByDomain(record.Domain);
            if (service == null)
                return;

            var internalIp = record.InternalIp;
            if (string.IsNullOrWhiteSpace(internalIp))
                return;

            results.Add(new LogOutput
            {
                Name = service.Name,
                InternalIp = internalIp
            });
        });

        return results.ToList();
    }
}