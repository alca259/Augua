using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;
using System.Net;

namespace SharedKernel.Utils;

public sealed class IPAddressHttpParser
{
    public static string GetIPAddress(HttpContext context)
    {
        if (context == null) return FormatOutput(string.Empty);

        var errors = new List<string>();
        if (context.Request?.Headers != null)
        {
            var headers = context.Request.Headers
                .Where(s => !string.IsNullOrEmpty(s.Key))
                .Select(s => new
                {
                    Key = s.Key.ToLower().Trim(),
                    Value = s.Value
                })
                .ToDictionary(k => k.Key, v => v.Value);

            foreach (var header in _headersToRead)
            {
                if (!headers.Any(a => a.Key.EqualsIgnoreCase(header))) continue;

                KeyValuePair<string, StringValues> foundHeader = headers.First(f => f.Key.EqualsIgnoreCase(header));

                (bool ok, string ip) = AssignIPAddress(foundHeader.Value.ToArray(), $"Header {header}", ref errors);
                if (ok) return FormatOutput(ip);
            }
            if (errors.Count > 0)
                errors.ForEach(error => Log.Debug(error));
        }

        if (context.Connection?.RemoteIpAddress == null) return FormatOutput(string.Empty);

        string possibleValue = context.Connection.RemoteIpAddress.ToString();
        (_, string ipx) = AssignIPAddress(possibleValue, "RemoteIpAddress", ref errors);
        return FormatOutput(ipx);
    }

    private static readonly HashSet<string> _headersToRead = new()
    {
        "x-original-host",
        "x-forwarded-for",
        "http_x_forwarded_for",
        "remote_addr",
        "x_forwarded_for",
        "http-x-forwarded-for",
        "remote-addr"
    };

    private static string FormatOutput(string value) => string.IsNullOrEmpty(value) ? string.Empty : $"ClientIP: \"{value}\"";

    private static (bool, string) AssignIPAddress(string[] possibleValue, string header, ref List<string> errors)
    {
        if (possibleValue == null) return (false, string.Empty);

        foreach (var item in possibleValue)
        {
            (bool ok, string ip) = AssignIPAddress(item, header, ref errors);
            if (ok) return (ok, ip);
        }

        return (false, string.Empty);
    }

    private static (bool, string) AssignIPAddress(string possibleValue, string header, ref List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(possibleValue)) return (false, string.Empty);

        var possibleValues = possibleValue.Split(":");

        switch (possibleValues.Length)
        {
            case 0: possibleValue = null; break;
            case 1: possibleValue = possibleValues[0]; break;
            default: return AssignIPAddress(possibleValues, header, ref errors);
        }
        if (!IPAddress.TryParse(possibleValue, out var possibleIPAddress))
        {
            errors.Add($"{header}: {possibleValue} is not a valid IP address.");
            return (false, string.Empty);
        }

        return (true, possibleIPAddress.ToString());
    }
}
