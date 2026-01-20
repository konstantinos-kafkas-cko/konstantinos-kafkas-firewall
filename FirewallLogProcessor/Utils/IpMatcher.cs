using System.Net;

namespace FirewallLogProcessor.Utils;

public static class IpMatcher
{
    public static bool Matches(string ip, string rule)
    {
        // rule can be "1.2.3.4" or "1.2.3.0/24"
        if (rule.Contains('/'))
            return MatchesCidr(ip, rule);

        return ip == rule;
    }

    private static bool MatchesCidr(string ip, string cidr)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2) return false;

        if (!IPAddress.TryParse(parts[0], out var baseIp)) return false;
        if (!IPAddress.TryParse(ip, out var targetIp)) return false;
        if (!int.TryParse(parts[1], out var prefixLength)) return false;

        // Only IPv4 for this assignment
        var baseBytes = baseIp.GetAddressBytes();
        var targetBytes = targetIp.GetAddressBytes();
        if (baseBytes.Length != 4 || targetBytes.Length != 4) return false;

        if (prefixLength < 0 || prefixLength > 32) return false;

        uint baseUint = ToUInt32(baseBytes);
        uint targetUint = ToUInt32(targetBytes);

        uint mask = prefixLength == 0 ? 0u : uint.MaxValue << (32 - prefixLength);
        return (baseUint & mask) == (targetUint & mask);
    }

    private static uint ToUInt32(byte[] bytes)
    {
        // bytes are in network order; convert to uint consistently
        return ((uint)bytes[0] << 24) |
               ((uint)bytes[1] << 16) |
               ((uint)bytes[2] << 8)  |
               bytes[3];
    }
}