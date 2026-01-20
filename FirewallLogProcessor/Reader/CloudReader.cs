using FirewallLogProcessor.Core;

namespace FirewallLogProcessor.Reader;

public class CloudReader : ICloudReader
{
    private readonly string _path;

    public CloudReader(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("CSV path must be provided", nameof(path));

        _path = path;
    }

    public IEnumerable<CloudEntry> Read()
    {
        using var reader = new StreamReader(_path);

        // Skip header
        var header = reader.ReadLine();
        if (header == null)
            yield break;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            if (parts.Length != 5)
                continue; // malformed line, skip safely

            yield return new CloudEntry()
            {
                Name = parts[0].Trim(),
                Domain = parts[1].Trim(),
                Risk = parts[2].Trim(),
                Country = parts[3].Trim(),
                GdprCompliant = parts[4].Trim()
                    .Equals("Yes", StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}