using Blazor.Cookies.Server.Services;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Blazor.Cookies.Tests.Server
{
    public class HttpContextCookieServiceTests
    {
        // HttpContextCookieService.GetAsync method will not be tested due to
        // the dependency on HttpContext.Request.Cookies which can only be
        // read.

        private (HttpContext, HttpContextCookieService) CreateTestDependencies()
        {
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            HttpContext httpContext = new DefaultHttpContext();
            httpContextAccessor.HttpContext = httpContext;
            HttpContextCookieService httpContextCookieService = new HttpContextCookieService(httpContextAccessor);

            return (httpContext, httpContextCookieService);
        }

        [Fact]
        public async Task GetAllAsync_WithCookies_ShouldReturnCookieIEnumerable()
        {
            DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire },
                new Cookie { Name = "userId", Value = "xyz789", Expires = cookieExpire },
                new Cookie { Name = "theme", Value = "dark", Expires = cookieExpire },
                new Cookie { Name = "cartItems", Value = "5", Expires = cookieExpire }
            };

            (var httpContext, var cookieService) = CreateTestDependencies();

            foreach(Cookie cookie in cookies)
            {
                await cookieService.SetAsync(cookie);
            }

            var resultCookiesValues = httpContext.Response.Headers.SetCookie;
            string resultCookies = resultCookiesValues.ToString();
            Assert.NotEmpty(resultCookies);
            for (int i = 0; i < cookies.Count; i++)
            {
                Assert.Contains($"{cookies[i].Name}={cookies[i].Value}", resultCookies);
            }
        }
        [Fact]
        public async Task GetAllAsync_WithEmptyValueCookies_ShouldReturnCookieIEnumerable()
        {
            DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie { Name = "sessionId", Value = "", Expires = cookieExpire },
                new Cookie { Name = "userId", Value = "", Expires = cookieExpire },
                new Cookie { Name = "theme", Value = "", Expires = cookieExpire },
                new Cookie { Name = "cartItems", Value = "", Expires = cookieExpire }
            };

            (var httpContext, var cookieService) = CreateTestDependencies();

            foreach (Cookie cookie in cookies) {
                await cookieService.SetAsync(cookie);
            }

            var resultCookiesValues = httpContext.Response.Headers.SetCookie;
            string resultCookies = resultCookiesValues.ToString();
            Assert.NotEmpty(resultCookies);
            for (int i = 0; i < cookies.Count; i++)
            {
                Assert.Contains($"{cookies[i].Name}={cookies[i].Value}", resultCookies);
            }
        }

        [Fact]
        public async Task SetAsync_WithCookies_ShouldReturnCookie()
        {
            (var httpContext, var cookieService) = CreateTestDependencies();

            DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire },
                new Cookie { Name = "userId", Value = "xyz789", Expires = cookieExpire },
                new Cookie { Name = "theme", Value = "dark", Expires = cookieExpire },
                new Cookie { Name = "cartItems", Value = "5", Expires = cookieExpire }
            };

            foreach(Cookie cookie in cookies)
            {
                await cookieService.SetAsync(cookie);
            }

            var responseCookies = httpContext.Response.Headers.SetCookie;
            for (int i = 0; i < responseCookies.Count; i++)
            {
                string responseCookie = responseCookies[i]!;
                string cookie = $"{cookies[i].Name}={cookies[i].Value}";
                Assert.NotEmpty(responseCookie);
                Assert.Contains(cookie, responseCookie);
            }
        }
        [Fact]
        public async Task SetAsync_WithEmptyCookies_ShouldReturnCookie()
        {
            (var httpContext, var cookieService) = CreateTestDependencies();

            DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie { Name = "sessionId", Value = "", Expires = cookieExpire },
                new Cookie { Name = "userId", Value = "", Expires = cookieExpire },
                new Cookie { Name = "theme", Value = "", Expires = cookieExpire },
                new Cookie { Name = "cartItems", Value = "", Expires = cookieExpire }
            };

            foreach (Cookie cookie in cookies)
            {
                await cookieService.SetAsync(cookie);
            }

            var responseCookies = httpContext.Response.Headers.SetCookie;
            for (int i = 0; i < responseCookies.Count; i++)
            {
                string responseCookie = responseCookies[i]!;
                string cookie = $"{cookies[i].Name}={cookies[i].Value}";
                Assert.NotEmpty(responseCookie);
                Assert.Contains(cookie, responseCookie);
            }
        }
    }
}
