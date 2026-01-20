namespace FirewallLogProcessor.Reader;

public class FirewallLogReader
{
    private readonly string _path;
    private readonly int _chunkSize;

    public FirewallLogReader(string path, int chunkSize)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("path must be provided", nameof(path));
        
        _path = path;
        _chunkSize = chunkSize;
    }

    public async IAsyncEnumerable<List<string>> ReadAsync()
    {
        using var reader = new StreamReader(_path);
        var chunk = new List<string>(_chunkSize);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            chunk.Add(line);
            if (chunk.Count == _chunkSize)
            {
                yield return chunk;
                chunk = new List<string>(_chunkSize);
            }
        }

        if (chunk.Count > 0)
            yield return chunk;
    }
}