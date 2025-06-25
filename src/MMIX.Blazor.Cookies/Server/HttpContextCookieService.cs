using MMIX.Blazor.Cookies.Patches;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace MMIX.Blazor.Cookies.Server;
public class HttpContextCookieService : ICookieService
{
    private readonly HttpContext _httpContext;
    private readonly Dictionary<string, Cookie> _requestCookies;
    private IHeaderDictionary ResponseHeaders { get; set; }

    public HttpContextCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;
        _requestCookies = _httpContext.Request.Cookies
            .Select(x => new Cookie(x.Key, x.Value))
            .ToDictionary(cookie => cookie.Name);
        ResponseHeaders = _httpContext.Response.Headers;
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
        Cookie cookie,
        SameSiteMode sameSiteMode,
        CancellationToken cancellationToken = default
    )
    {
        RemoveCookieIfExistsFromHeader(cookie.Name);
        AppendCookieToHttpContext(cookie, sameSiteMode);

        return Task.CompletedTask;
    }
    public Task SetAsync(
        string name,
        string value,
        CancellationToken cancellationToken = default
    )
    {
        RemoveCookieIfExistsFromHeader(name);
        _httpContext.Response.Cookies.Append(name, value);

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
        DateTime expires,
        SameSiteMode sameSiteMode,
        CancellationToken cancellationToken = default
    )
    {
        Cookie cookie = new Cookie
        {
            Name = name,
            Value = value,
            Expires = expires,
        };

        RemoveCookieIfExistsFromHeader(cookie.Name);
        AppendCookieToHttpContext(cookie, sameSiteMode);

        return Task.CompletedTask;
    }

    private void AppendCookieToHttpContext(Cookie cookie)
    {
        _httpContext.Response.Cookies.Append(
            cookie.Name,
            cookie.Value,
            new CookieOptions
            {
                Expires = cookie.Expires,
                Path = "/",
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = SameSiteMode.Lax
            }
        );
    }
    private void AppendCookieToHttpContext(
        Cookie cookie,
        SameSiteMode sameSiteMode
    )
    {
        _httpContext.Response.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions
        {
            Expires = cookie.Expires,
            Path = "/",
            HttpOnly = cookie.HttpOnly,
            Secure = cookie.Secure,
            SameSite = sameSiteMode
        });
    }

    private void RemoveCookieIfExistsFromHeader(string name)
    {
        List<string?> responseCookies = ResponseHeaders[HeaderNames.SetCookie].ToList();
        
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
            ResponseHeaders[HeaderNames.SetCookie] = responseCookies.ToArray();
        }
    }

    public Task RemoveAsync(string name, CancellationToken cancellationToken = default)
    {
        // deletes cookie from response request
        RemoveCookieIfExistsFromHeader(name);

        if (_requestCookies.Remove(name))
        {
            // deletes cookie from client
            _httpContext.Response.Cookies.Delete(name);
        }

        return Task.CompletedTask;
    }
}
