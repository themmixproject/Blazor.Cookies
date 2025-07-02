using Microsoft.JSInterop;
using Moq;
using System.Net;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace MMIX.Blazor.Cookies.Tests.Patches;

internal class TestJSRuntime : IJSRuntime
{
    private string _cookies = "";

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        if (Regex.Match(identifier, "eval$").Success && args is { Length: 1})
        {
            string command = args[0]?.ToString() ?? "";
            ParseCommand(command);
        }
        return ValueTask.FromResult((TValue) (object) null);
    }

    private void ParseCommand(string command)
    {
        if (Regex.IsMatch(command, @"document\.cookie$")) {
            return _cookies;
        }
        if (Regex.IsMatch(command, @"document\.cookie\s*=\s*"))
        {
            var extractionMatch= Regex.Match(command, @"/document\.cookie\s*=\s*[\'](.*)[\']");
            var cookieString = extractionMatch.Groups[1].Value;
            var cookieNameValuePair = cookieString.Split(';')[0].Trim().Split("=");
            _cookies += cookieNameValuePair[0] + "=" + cookieNameValuePair[1] + "; ";
            return cookieString;
        }
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}
