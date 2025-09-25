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
     
        /// TODO: refactor initialization of _requestCookies so that
        ///       it's index-based instead of name-based
        _requestCookies = _httpContext.Request.Cookies
            .Select(x => new Cookie(x.Key, x.Value))
            .ToDictionary(cookie => cookie.Name);
    }

    public Task<IEnumerable<Cookie>> GetAllAsync()
    {
        return Task.FromResult(
            _requestCookies.Select(c => c.Value).ToList().AsEnumerable()
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
        RemoveCookieIfExistsFromHeader(cookie.Name);
        AppendCookieToHttpContext(cookie);

        return Task.CompletedTask;
    }

    public Task SetAsync(
        string name,
        string value,
        CancellationToken cancellationToken = default
    )
    {
        RemoveCookieIfExistsFromHeader(name);
        AppendCookieToHttpContext(new Cookie(name, value));

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

        RemoveCookieIfExistsFromHeader(cookie.Name);
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
        RemoveCookieIfExistsFromHeader(name);
        _httpContext.Response.Cookies.Append(name, value, cookieOptions);

        return Task.CompletedTask;
    }

    private void AppendCookieToHttpContext(Cookie cookie)
    {
        bool isSession = cookie.Expires.ToUniversalTime() == DateTimeOffset.MinValue;
        _httpContext.Response.Cookies.Append(
            cookie.Name,
            cookie.Value,
            new CookieOptions
            {
                Expires = isSession ? null : cookie.Expires.ToUniversalTime(),
                Path = string.IsNullOrEmpty(cookie.Path) ? "/" : cookie.Path,
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = SameSiteMode.Lax
            }
        );
    }

    private void RemoveCookieIfExistsFromHeader(string name)
    {
        IHeaderDictionary responseHeaders = _httpContext.Response.Headers;
        List<string?> responseCookies = responseHeaders[HeaderNames.SetCookie].ToList();

        bool isRemoved = false;

        for (int i = 0; i < responseCookies.Count; i++)
        {
            bool isMatchedCookie = responseCookies[i]!.StartsWith($"{name}=");
            if (isMatchedCookie)
            {
                responseCookies.RemoveAt(i);
                isRemoved = true;
                break;
            }
        }

        if (isRemoved)
        {
            responseHeaders[HeaderNames.SetCookie] = responseCookies.ToArray();
        }
    }

    public Task RemoveAsync(string name, CancellationToken cancellationToken = default)
    {
        RemoveCookieIfExistsFromHeader(name);
        if (_requestCookies.Remove(name))
        {
            _httpContext.Response.Cookies.Delete(name);
        }

        return Task.CompletedTask;
    }
}
