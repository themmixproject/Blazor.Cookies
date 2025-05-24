using Blazor.Cookies.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Blazor.Cookies.Server.Services
{

    internal class HttpContextCookieService : ICookieService
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

        public Task SetAsync(Cookie cookie, SameSiteMode sameSiteMode, CancellationToken cancellationToken)
        {
            if (cookie.Secure && !cookie.HttpOnly)
            {
                throw new InvalidOperationException("Unable to set a cookie: Secure cookies must also be HttpOnly.");
            }

            RemovePending(cookie.Name);

            _httpContext.Response.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions
            {
                Expires = cookie.Expires,
                Path = "/",
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = sameSiteMode
            });

            return Task.CompletedTask;
        }

        private void RemovePending(string key)
        {
            List<string?> cookieValues = ResponseHeaders
                .SingleOrDefault(header => header.Key == "Set-Cookie")
                .Value
                .ToList<string?>();

            foreach (string? cookieValue in cookieValues)
            {
                if (string.IsNullOrEmpty(cookieValue)) { continue; }
                if (!cookieValue.StartsWith($"{key}=")) { continue; }

                cookieValues.Remove(cookieValue);
                ResponseHeaders.SetCookie = new([.. cookieValues]);
            }
        }
    }
}
