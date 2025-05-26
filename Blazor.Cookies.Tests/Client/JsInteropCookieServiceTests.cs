using Blazor.Cookies.Client.Services;
using Blazor.Cookies.Interfaces;
using Microsoft.JSInterop;
using Moq;
using System.Net;

namespace Blazor.Cookies.Tests.Client
{
    public class JsInteropCookieServiceTests
    {
        private JsInteropCookieService CreateMockCookieService(string jsInteropReturnValue)
        {
            var jsRuntime = new Mock<IJSRuntime>();
            jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
                .ReturnsAsync(jsInteropReturnValue);
            return new JsInteropCookieService(jsRuntime.Object);
        }

        private static string ToJsCookieString(IEnumerable<Cookie> cookies)
        {
            return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        }
      
        [Fact]
        public async Task GetAllAsync_WithCookies_ReturnsIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>
            {
                new Cookie("sessionId", "ei34jdh"),
                new Cookie("userId", "xyz789"),
                new Cookie("theme", "dark"),
                new Cookie("cartItems", "5")
            };
            ICookieService cookieService = CreateMockCookieService(
                ToJsCookieString(cookies)
            );
            
            var resultCookies = await cookieService.GetAllAsync();
            Assert.NotEmpty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithEmptyCookies__ReturnsIEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>();
            ICookieService cookieService = CreateMockCookieService(
                ToJsCookieString(cookies)
            );

            var resultCookies = await cookieService.GetAllAsync();
            Assert.Empty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
        [Fact]
        public async Task GetAllAsync_WithNullCookies_ReturnsIEnumerable()
        {
            List<Cookie>? cookies = null;
            ICookieService cookieService = CreateMockCookieService(null);

            var resultCookies = await cookieService.GetAllAsync();
            Assert.Empty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
        }
    }
}
