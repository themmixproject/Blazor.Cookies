using Blazor.Cookies.Client.Services;
using Blazor.Cookies.Interfaces;
using Microsoft.JSInterop;
using Moq;
using System.Net;

namespace Blazor.Cookies.Tests.Client
{
    public class JsInteropCookieServiceTests
    {
        private static string ToJsCookieString(IEnumerable<Cookie>? cookies)
        {
            if (cookies == null) { return string.Empty; }
            if (cookies.Count() == 0) { return string.Empty; }

            return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        }

        private static async Task<IEnumerable<Cookie>> GetAllAsyncFromCookieService(IEnumerable<Cookie> cookies)
        {
            var jsRuntime = new Mock<IJSRuntime>();
            jsRuntime.Setup(x => x.InvokeAsync<string?>("eval", It.IsAny<object[]>()))
                .ReturnsAsync(ToJsCookieString(cookies));

            JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime.Object);
            return await cookieService.GetAllAsync();
        }
      
        [Fact]
        public async Task GetAllAsync_WithCookies_ShouldReturnCookieIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie("sessionId", "ei34jdh"),
                new Cookie("userId", "xyz789"),
                new Cookie("theme", "dark"),
                new Cookie("cartItems", "5")
            };

            var resultCookies = await GetAllAsyncFromCookieService(cookies);
            Assert.NotEmpty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithCookie_ShouldReturnCookieIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie("sessionId", "ei34jdh")
            };

            var resultCookies = await GetAllAsyncFromCookieService(cookies);
            Assert.NotEmpty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithEmptyCookie_ShouldReturnCookieIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie("sessionId", "ei34jdh")
            };

            var resultCookies = await GetAllAsyncFromCookieService(cookies);
            Assert.NotEmpty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithEmptyValueCookies_ShouldReturnCookieIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie("sessionId", ""),
                new Cookie("userId", ""),
                new Cookie("theme", ""),
                new Cookie("cartItems", "")
            };

            var resultCookies = await GetAllAsyncFromCookieService(cookies);
            Assert.NotEmpty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithEmptyCookies__ShouldReturnCookieIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>();

            var resultCookies = await GetAllAsyncFromCookieService(cookies);
            Assert.Empty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithNullCookies_ShouldReturnCookieIEnumerable()
        {
            var resultCookies = await GetAllAsyncFromCookieService(null);
            Assert.Empty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
        }
    }
}
