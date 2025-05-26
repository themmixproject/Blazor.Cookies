using Blazor.Cookies.Client.Services;
using Blazor.Cookies.Interfaces;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Cookies.Tests.Client
{
    public class JsInteropCookieServiceTests
    {
        private ICookieService CreateGetAllAsyncCookieService(string returnValue)
        {
            var jsRuntime = new Mock<IJSRuntime>();
            jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
                .ReturnsAsync(returnValue);
            return new JsInteropCookieService(jsRuntime.Object);
        }

        public static string ToJsCookieString(IEnumerable<Cookie> cookies)
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
            ICookieService cookieService = CreateGetAllAsyncCookieService(
                ToJsCookieString(cookies)
            );
            
            var resultCookies = await cookieService.GetAllAsync();

            Assert.NotEmpty(resultCookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
            Assert.Equal(cookies, resultCookies);
        }
    }
}
