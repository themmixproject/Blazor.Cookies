using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace MMIX.Blazor.Cookies.Tests.Patches;

internal class TestJSRuntime : IJSRuntime
{
    private Dictionary<string, string> _cookies = new Dictionary<string, string>();

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
        if (Regex.Match(command, @"document\.cookie$").Success) { }
        if (Regex.Match(command, @"document\.cookie\s=\s").Success) { }
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}
