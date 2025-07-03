using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace MMIX.Blazor.Cookies.Tests.Patches;

internal class TestJSRuntime : IJSRuntime
{
    private string _cookies = "";

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        if (Regex.IsMatch(identifier, "eval$") && args is { Length: 1})
        {
            string command = args[0]?.ToString() ?? "";
            var commandResult = ParseCommand(command);

            if (commandResult is null)
            {
                return ValueTask.FromResult(default(TValue)!);
            }

            if (commandResult is TValue value)
            {
                return ValueTask.FromResult(value);
            }

            return ValueTask.FromResult(TryTypeConverstion<TValue>(commandResult));
        }

        return ValueTask.FromResult(default(TValue)!);
    }

    private object? ParseCommand(string command)
    {
        if (Regex.IsMatch(command, @"document\.cookie$")) {
            return _cookies;
        }
        if (Regex.IsMatch(command, @"document\.cookie\s*=\s*"))
        {
            var cookieString = ExtractCookieStringFromCommand(command);
            var cookieNameValuePair = cookieString.Split(';')[0].Trim().Split("=");
            _cookies += cookieNameValuePair[0] + "=" + cookieNameValuePair[1] + "; ";
            return cookieString;
        }

        return null;
    }

    private string ExtractCookieStringFromCommand(string command)
    {
        Match ExtractionMatch = Regex.Match(command, @"document\.cookie\s*=\s*\'(.*)\'");
        string cookieString = ExtractionMatch.Groups[1].Value;
        return cookieString;
    }
    
    private TValue TryTypeConverstion<TValue>(object value)
    {
        try
        {
            return (TValue)Convert.ChangeType(value, typeof(TValue));
        }
        catch
        {
            throw new InvalidCastException($"Cannot cast result of type {value.GetType()} to {typeof(TValue)}");
        }
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}
