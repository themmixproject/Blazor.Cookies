using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Net;

namespace MMIX.Blazor.Cookies.Client;

public class JSInteropCookieService(IJSRuntime JSRuntime) : ICookieService
{
    private const string CookieFlagsExplainMessage = "Setting HttpOnly or Secure cookies is only possible when using Satic SSR render mode.";
    private const string NotSupportedSuffix = "are not supported in this rendering environment";
    private const string HttpOnlyFlagExceptionMessage = $"HttpOnly cookies {NotSupportedSuffix}. {CookieFlagsExplainMessage}";
    private const string SecureFlagExceptionMessage = $"Secure cookies {NotSupportedSuffix}. {CookieFlagsExplainMessage}";

    public async Task<IEnumerable<Cookie>> GetAllAsync()
    {
        var raw = await JSRuntime.InvokeAsync<string>("eval", "document.cookie");
        if (string.IsNullOrWhiteSpace(raw)) { return []; }

        return raw.Split("; ").Select(ToCookie);
    }

    private Cookie ToCookie(string raw)
    {
        var parts = raw.Split("=", 2);;
        return new Cookie { Name = parts[0], Value = parts[1] };
    }

    public async Task<Cookie?> GetAsync(string name)
    {
        var cookies = await GetAllAsync();
        return cookies.FirstOrDefault(c => c.Name == name);
    }

    public async Task SetAsync(
        Cookie cookie,
        CancellationToken cancellationToken = default
    )
    {
        ValidateCookie(cookie);
        await ExecuteSetCookieJavaScriptInteropAsync(
            cookie,
            cancellationToken
        );
    }

    public async Task SetAsync(
        IEnumerable<Cookie> cookies,
        CancellationToken cancellationToken = default
    )
    {
        foreach (Cookie cookie in cookies)
        {
            ValidateCookie(cookie);
            await ExecuteSetCookieJavaScriptInteropAsync(
                cookie,
                cancellationToken
            );
        }
    }

    public async Task SetAsync(
        string name,
        string value,
        CancellationToken cancellationToken = default
    )
    {
        Cookie cookie = new Cookie(name, value);
        ValidateCookie(cookie);
        await ExecuteSetCookieJavaScriptInteropAsync(cookie, cancellationToken);
    }
    public async Task SetAsync(
        string name,
        string value,
        DateTime expires,
        CancellationToken cancellationToken = default
    )
    {
        Cookie cookie = new Cookie
        {
            Name = name,
            Value = value,
            Expires = expires,
        };
        ValidateCookie(cookie);
        await ExecuteSetCookieJavaScriptInteropAsync(cookie, cancellationToken);
    }

    public async Task SetAsync(
        string name,
        string value,
        CookieOptions cookieOptions,
        CancellationToken cancellationToken = default
    )
    {
        Cookie cookie = new Cookie(name, value);
        ValidateCookie(cookie);
        await ExecuteSetCookieJavaScriptInteropAsync(
            name,
            value,
            cookieOptions,
            cancellationToken
        );
    }

    private void ValidateCookie(Cookie cookie)
    {
        if (cookie.HttpOnly)
        {
            throw new InvalidOperationException(HttpOnlyFlagExceptionMessage);
        }
        if (cookie.Secure)
        {
            throw new InvalidOperationException(SecureFlagExceptionMessage);
        }
    }

    private async Task ExecuteSetCookieJavaScriptInteropAsync(
        Cookie cookie,
        CancellationToken cancellationToken
    )
    {
        string command =
            $"document.cookie = '{cookie.Name}={cookie.Value}; " +
            $"{(cookie.Expires == default ? "" : $"expires={cookie.Expires:R}; ")}" +
            $"path={(string.IsNullOrEmpty(cookie.Path) ? '/' : cookie.Path)}" +
            $"'";

        await JSRuntime.InvokeVoidAsync("eval", cancellationToken, command);
    }

    public async Task ExecuteSetCookieJavaScriptInteropAsync(
        string name,
        string value,
        CookieOptions cookieOptions,
        CancellationToken cancellationToken
    )
    {
        string command =
            $"document.cookie = '{name}={value}; " +
            $"{(cookieOptions.Expires == default ? "" : $"expires={cookieOptions.Expires:R}; ")}" +
            $"path={(string.IsNullOrEmpty(cookieOptions.Path) ? '/' : cookieOptions.Path)}; " +
            $"SameSite={(cookieOptions.SameSite == default ? "" : $"Samesite={cookieOptions.SameSite}")}" +
            "'";

        await JSRuntime.InvokeVoidAsync("eval", cancellationToken, command);
    }

    public async Task RemoveAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(name)) { throw new Exception("Name is required when removing a cookie."); }

        string command = $"document.cookie = '{name}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/'";
        await JSRuntime.InvokeVoidAsync("eval", cancellationToken, command);
    }

    public async Task RemoveAllAsync(CancellationToken cancellationToken = default)
    {
        string command = "document.cookie.split(';').forEach(cookie => {" +
                         "let equalsPos = cookie.indexOf('=');" +
                         "let name = equalsPos > -1 ? cookie.substring(0, equalsPos) : cookie;" +
                         "document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 GMT'" +
                         "})";

        await JSRuntime.InvokeVoidAsync("eval", cancellationToken, command);
    }
}
