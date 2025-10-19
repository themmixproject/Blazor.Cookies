using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace MMIX.Blazor.Cookies.Patches.JSInterop;

public class VirtualJSRuntime : IJSRuntime
{
    private readonly Jint.Engine _engine;
    public VirtualJSRuntime()
    {
        _engine = new Jint.Engine();

        ReadCookieJS();
    }

    private void ReadCookieJS()
    {
        FileStream fileStream = new FileStream("cookie.js", FileMode.Open, FileAccess.Read);
        using (StreamReader reader = new StreamReader(fileStream))
        {
            string fileContents = reader.ReadToEnd();
            _engine.Execute(fileContents);
        }
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        if (Regex.IsMatch(identifier, "eval$") && args is { Length: 1 })
        {
            var commandResult = _engine.Invoke(identifier, args).ToObject();
            if (commandResult is TValue value)
            {
                return ValueTask.FromResult(value);
            }
        }

        return ValueTask.FromResult(default(TValue)!);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}
