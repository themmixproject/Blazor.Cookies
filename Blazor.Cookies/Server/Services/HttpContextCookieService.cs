using Blazor.Cookies.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
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

        public Task SetAsync(Cookie cookie, CancellationToken cancellationToken)
        {
            ValidateCookie(cookie);
            RemoveCookieIfExistsFromHeader(cookie.Name);
            AppendCookieToHttpContext(cookie);

            return Task.CompletedTask;
        }
        public Task SetAsync(Cookie cookie, SameSiteMode sameSiteMode, CancellationToken cancellationToken)
        {
            ValidateCookie(cookie);
            RemoveCookieIfExistsFromHeader(cookie.Name);
            AppendCookieToHttpContext(cookie, sameSiteMode);

            return Task.CompletedTask;
        }
        public Task SetAsync(string name, string value, CancellationToken cancellationToken)
        {
            Cookie cookie = new Cookie(name, value);
            ValidateCookie(cookie);
            RemoveCookieIfExistsFromHeader(cookie.Name);
            AppendCookieToHttpContext(cookie);

            return Task.CompletedTask;
        }
        public Task SetAsync(string name, string value, DateTime expires, CancellationToken cancellationToken)
        {
            Cookie cookie = new Cookie
            {
                Name = name,
                Value = value,
                Expires = expires
            };
            ValidateCookie(cookie);
            RemoveCookieIfExistsFromHeader(cookie.Name);
            AppendCookieToHttpContext(cookie);

            return Task.CompletedTask;
        }
        public Task SetAsync(string name, string value, DateTime expires, SameSiteMode sameSiteMode, CancellationToken cancellationToken)
        {
            Cookie cookie = new Cookie
            {
                Name = name,
                Value = value,
                Expires = expires,
            };
            ValidateCookie(cookie);
            RemoveCookieIfExistsFromHeader(cookie.Name);
            AppendCookieToHttpContext(cookie);

            return Task.CompletedTask;
        }

        private void ValidateCookie(Cookie cookie)
        {
            if (cookie.Secure && !cookie.HttpOnly)
            {
                throw new InvalidOperationException("Unable to set a cookie: Secure cookies must also be HttpOnly.");
            }
        }

        private void AppendCookieToHttpContext(Cookie cookie)
        {
            _httpContext.Response.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions
            {
                Expires = cookie.Expires,
                Path = "/",
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = SameSiteMode.Unspecified
            });
        }
        private void AppendCookieToHttpContext(Cookie cookie, SameSiteMode sameSiteMode)
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
            List<string?> cookieValues = ResponseHeaders
                .SingleOrDefault(header => header.Key == "Set-Cookie")
                .Value
                .ToList<string?>();

            foreach (string? cookieValue in cookieValues)
            {
                if (string.IsNullOrEmpty(cookieValue)) { continue; }
                if (!cookieValue.StartsWith($"{name}=")) { continue; }

                cookieValues.Remove(cookieValue);
                ResponseHeaders.SetCookie = new(cookieValues.ToArray());
            }
        }

        public Task RemoveAsync(string name, CancellationToken cancellationToken)
        {
            if (_requestCookies.Remove(name))
            {
                _httpContext.Response.Cookies.Delete(name);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(CancellationToken cancellationToken)
        {
            foreach(var cookie in _requestCookies)
            {
                _httpContext.Response.Cookies.Delete(cookie.Key);
            }
            _requestCookies.Clear();

            return Task.CompletedTask;
        }
    }
}
