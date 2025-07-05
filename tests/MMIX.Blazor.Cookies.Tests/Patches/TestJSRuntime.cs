using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using Xunit.Sdk;

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
            var cookieNameValuePair = ExtractNameValuePairFromCommand(cookieString);
            AddKeyValueToCookies(cookieNameValuePair);

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

    private string ExtractNameValuePairFromCommand(string command)
    {
        string[] cookieParts = command.Split(";");
        foreach (string cookiePart in cookieParts)
        {
            if (string.IsNullOrEmpty(cookiePart)) { continue; }

            string[] cookiePartSplit = cookiePart.Split("=", 2);
            string key = cookiePartSplit[0].Trim();
            string value = cookiePartSplit[1].Trim();

            if (isCustomKey(key))
            {
                return $"{key}={value}";
            }
        }

        return "";
    }

    private bool isCustomKey(string key)
    {
        string lowerCaseKey = key.ToLower();
        string[] validKeys = {
            "domain",
            "expires",
            "max-age",
            "partitioned",
            "path",
            "samesite"
        };

        return validKeys.Contains(lowerCaseKey);
    }

    private void AddKeyValueToCookies(string keyValuePair)
    {
        bool cookiesAreSet = !string.IsNullOrEmpty(_cookies);
        if (cookiesAreSet)
        {
            _cookies += "; ";
        }

        _cookies += keyValuePair;
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
