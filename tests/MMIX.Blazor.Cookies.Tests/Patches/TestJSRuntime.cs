using Microsoft.JSInterop;

namespace MMIX.Blazor.Cookies.Tests.Patches;

internal class TestJSRuntime : IJSRuntime
{
    private Dictionary<string, string> _cookies = new Dictionary<string, string>();

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return ValueTask.FromResult((TValue) (object) null);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}
