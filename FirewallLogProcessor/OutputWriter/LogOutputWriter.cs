using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.OutputWriter;

public class LogOutputWriter
{
    public void Output(List<LogOutput> logOutputs)
    {
        foreach (var logOutput in logOutputs)
        {
            Console.WriteLine($"{logOutput.Name} : {logOutput.InternalIp}");
        }
    }
    
    public void Output(Dictionary<string, HashSet<string>> aggregation)
    {
        foreach (var (service, ips) in aggregation.OrderBy(x => x.Key))
        {
            Console.WriteLine($"{service}: {string.Join(", ", ips.OrderBy(x => x))}");
        }
    }
}