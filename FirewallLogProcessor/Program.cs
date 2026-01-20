// See https://aka.ms/new-console-template for more information

using FirewallLogProcessor.Core;
using FirewallLogProcessor.Filters;
using FirewallLogProcessor.OutputWriter;
using FirewallLogProcessor.Processor;
using FirewallLogProcessor.Reader;

var cloudReader = new CloudReader("ServiceDBv1.csv");
var cloudEntries = cloudReader.Read().ToList();
var cloudServiceIndex = new CloudServiceIndex(cloudEntries);
var logReader = new FirewallLogReader("firewall.log", 1000);
var logParser = new FirewallLogParser();
var userFilter = new IncludeUserRegexFilter([
    @"^john@acme\.com$"
]);
var dnsResolver = new DnsResolver(10000);
var logProcessor = new LogProcessor(logParser, new List<IFilter>(){userFilter}, dnsResolver, cloudServiceIndex);
var outputWriter = new LogOutputWriter();

var aggregation = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
await foreach (var chunk in logReader.ReadAsync())
{
    var outputs = await logProcessor.ProcessAsync(chunk, CancellationToken.None);

    foreach (var o in outputs)
    {
        if (!aggregation.TryGetValue(o.Name, out var set))
        {
            set = new HashSet<string>(StringComparer.Ordinal);
            aggregation[o.Name] = set;
        }
        set.Add(o.InternalIp.Trim());
    }
}
outputWriter.Output(aggregation);