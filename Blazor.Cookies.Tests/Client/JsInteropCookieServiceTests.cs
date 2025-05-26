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
        [Fact]
        public async Task GetAllAsync_WithCookies_ReturnsIEnumerable()
        {
            ICookieService cookieService = Get_GetAllAsync_WithCookies_CookieService();
            
            var cookies = await cookieService.GetAllAsync();

            Assert.NotEmpty(cookies);
            Assert.IsAssignableFrom<IEnumerable<Cookie>>(cookies);
        }
        private ICookieService Get_GetAllAsync_WithCookies_CookieService()
        {
            var jsRuntime = new Mock<IJSRuntime>();
            jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
                .ReturnsAsync("sessionId=abc123; userId=xyz789; theme=dark; cartItems=5");
            return new JsInteropCookieService(jsRuntime.Object);
        }


    }
}
