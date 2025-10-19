using MMIX.Blazor.Cookies.Patches;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Buffers;

namespace MMIX.Blazor.Cookies.Server;

public class HttpContextCookieService : ICookieService
{
    private readonly HttpContext _httpContext;
    private readonly Dictionary<string, Cookie> _requestCookies;

    public HttpContextCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;

        _requestCookies = _httpContext.Request.Cookies
            .Select(x => new Cookie(x.Key, x.Value))
            .ToDictionary(cookie => cookie.Name);
    }

    public Task<IEnumerable<Cookie>> GetAllAsync()
    {
        return Task.FromResult(
            _requestCookies.Values.AsEnumerable()
        );
    }

    public Task<Cookie?> GetAsync(string name)
    {
        if (_requestCookies.TryGetValue(name, out var cookie))
        {
            return Task.FromResult<Cookie?>(cookie);
        }

        return Task.FromResult<Cookie?>(null);
    }

    public Task SetAsync(
        Cookie cookie,
        CancellationToken cancellationToken = default
    )
    {
        RemovePendingCookieFromSetCookieHeader(cookie.Name);
        AppendCookieToHttpContext(cookie);

        return Task.CompletedTask;
    }

    public Task SetAsync(
        IEnumerable<Cookie> cookies,
        CancellationToken cancellationToken = default
    )
    {
        foreach (Cookie cookie in cookies)
        {
            RemovePendingCookieFromSetCookieHeader(cookie.Name);
            AppendCookieToHttpContext(cookie);
        }

        return Task.CompletedTask;
    }

    public Task SetAsync(
        string name,
        string value,
        CancellationToken cancellationToken = default
    )
    {
        RemovePendingCookieFromSetCookieHeader(name);
        
        AppendCookieToHttpContext(new Cookie{
            Name = name,
            Value = value,

            // Setting expires to DateTime.MinValue will create a session cookie
            Expires = DateTime.MinValue
        });

        return Task.CompletedTask;
    }

    public Task SetAsync(
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
            Expires = expires
        };

        RemovePendingCookieFromSetCookieHeader(cookie.Name);
        AppendCookieToHttpContext(cookie);

        return Task.CompletedTask;
    }

    public Task SetAsync(
        string name,
        string value,
        CookieOptions cookieOptions,
        CancellationToken cancellationToken = default
    )
    {
        RemovePendingCookieFromSetCookieHeader(name);
        _httpContext.Response.Cookies.Append(name, value, cookieOptions);

        return Task.CompletedTask;
    }

    private void AppendCookieToHttpContext(Cookie cookie)
    {
        bool isSessionCookie = cookie.Expires == DateTime.MinValue;
        _httpContext.Response.Cookies.Append(
            cookie.Name,
            cookie.Value,
            new CookieOptions
            {
                Expires = isSessionCookie ? null : cookie.Expires.ToUniversalTime(),
                Path = string.IsNullOrEmpty(cookie.Path) ? "/" : cookie.Path,
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = SameSiteMode.Unspecified
            }
        );
    }

    private void RemovePendingCookieFromSetCookieHeader(string name)
    {
        IHeaderDictionary responseHeaders = _httpContext.Response.Headers;
        List<string?> responseCookies = responseHeaders[HeaderNames.SetCookie].ToList();

        for (int i = 0; i < responseCookies.Count; i++)
        {
            bool isMatchedCookie = responseCookies[i]!.StartsWith(
                $"{name}=",
                StringComparison.Ordinal
            );
            if (isMatchedCookie)
            {
                responseCookies.RemoveAt(i);
                responseHeaders[HeaderNames.SetCookie] = responseCookies.ToArray();
                break;
            }
        }
    }

    public Task RemoveAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        // Remove cookie from set-cookie header if cookie is added during
        // request
        RemovePendingCookieFromSetCookieHeader(name);

        // If cookie is in request, add set-cookie header to expire cookie
        if (_requestCookies.Remove(name))
        {
            _httpContext.Response.Cookies.Delete(name);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAllAsync(CancellationToken cancellationToken = default)
    {
        var keys = _requestCookies.Keys.ToArray();
        foreach (var key in keys)
        {
            // Remove cookie from set-cookie header if cookie is added during
            // request
            RemovePendingCookieFromSetCookieHeader(key);

            // Add set-cookie header to expire cookie
            _httpContext.Response.Cookies.Delete(key);
        }

        _requestCookies.Clear();

        return Task.CompletedTask;
    }
}
