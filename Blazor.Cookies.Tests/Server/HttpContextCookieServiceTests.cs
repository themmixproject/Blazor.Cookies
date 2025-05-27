using Blazor.Cookies.Server.Services;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Blazor.Cookies.Tests.Server
{
    public class HttpContextCookieServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WithCookies_ShouldReturnCookieIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie
                {
                    Name = "sessionId",
                    Value = "ei34jdh",
                    Expires = DateTime.UtcNow.AddDays(1),
                },
                new Cookie
                {
                    Name = "userId",
                    Value = "xyz789",
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new Cookie
                {
                    Name = "theme",
                    Value = "dark",
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new Cookie
                {
                    Name = "cartItems",
                    Value = "5",
                    Expires = DateTime.UtcNow.AddDays(1)
                }
            };

            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            HttpContext httpContext = new DefaultHttpContext();
            httpContextAccessor.HttpContext = httpContext;
            HttpContextCookieService httpContextCookieService = new HttpContextCookieService(httpContextAccessor);

            foreach(Cookie cookie in cookies)
            {
                await httpContextCookieService.SetAsync(cookie);
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
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie
                {
                    Name = "sessionId",
                    Value = "",
                    Expires = DateTime.UtcNow.AddDays(1),
                },
                new Cookie
                {
                    Name = "userId",
                    Value = "",
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new Cookie
                {
                    Name = "theme",
                    Value = "",
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new Cookie
                {
                    Name = "cartItems",
                    Value = "",
                    Expires = DateTime.UtcNow.AddDays(1)
                }
            };

            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            HttpContext httpContext = new DefaultHttpContext();
            httpContextAccessor.HttpContext = httpContext;
            HttpContextCookieService httpContextCookieService = new HttpContextCookieService(httpContextAccessor);

            foreach (Cookie cookie in cookies)
            {
                await httpContextCookieService.SetAsync(cookie);
            }

            var resultCookiesValues = httpContext.Response.Headers.SetCookie;
            string resultCookies = resultCookiesValues.ToString();
            Assert.NotEmpty(resultCookies);
            for (int i = 0; i < cookies.Count; i++)
            {
                Assert.Contains($"{cookies[i].Name}={cookies[i].Value}", resultCookies);
            }
        }
    }
}
