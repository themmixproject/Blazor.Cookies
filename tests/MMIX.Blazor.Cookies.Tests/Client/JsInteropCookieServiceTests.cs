using MMIX.Blazor.Cookies.Client;
using MMIX.Blazor.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using System.Net;

namespace MMIX.Blazor.Cookies.Tests.Client;
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
        var jsRuntime = GetMockJSRuntime(cookies);

        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);
        return await cookieService.GetAllAsync();
    }
    private static IJSRuntime GetMockJSRuntime(IEnumerable<Cookie> cookies)
    {
        Mock<IJSRuntime> jsRuntime = new Mock<IJSRuntime>();
        jsRuntime.Setup(jsRuntime => jsRuntime.InvokeAsync<string?>("eval", It.IsAny<object[]>()))
            .ReturnsAsync(ToJsCookieString(cookies));

        return jsRuntime.Object;
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
            new Cookie("sessionId", "")
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

    [Fact]
    public async Task GetAsync_WithCookies_ShouldReturnCookie()
    {
        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie("sessionId", "ei34jdh"),
            new Cookie("userId", "xyz789"),
            new Cookie("theme", "dark"),
            new Cookie("cartItems", "5")
        };

        var jsRuntime = GetMockJSRuntime(cookies);
        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);
        
        var sessionIdCookie = await cookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);

        var userIdCookie = await cookieService.GetAsync("userId");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "userId"), userIdCookie);

        var themeCookie = await cookieService.GetAsync("theme");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "theme"), themeCookie);

        var cartItemsCookie = await cookieService.GetAsync("cartItems");
        Assert.NotNull(cartItemsCookie);
        Assert.Equal(cookies.First(c => c.Name == "cartItems"), cartItemsCookie);
    }
    [Fact]
    public async Task GetAsync_WithEmptyValueCookies_ShouldReturnCookie()
    {
        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie("sessionId", ""),
            new Cookie("userId", ""),
            new Cookie("theme", ""),
            new Cookie("cartItems", "")
        };

        var jsRuntime = GetMockJSRuntime(cookies);
        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);

        var sessionIdCookie = await cookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);

        var userIdCookie = await cookieService.GetAsync("userId");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "userId"), userIdCookie);

        var themeCookie = await cookieService.GetAsync("theme");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "theme"), themeCookie);

        var cartItemsCookie = await cookieService.GetAsync("cartItems");
        Assert.NotNull(cartItemsCookie);
        Assert.Equal(cookies.First(c => c.Name == "cartItems"), cartItemsCookie);
    }
    [Fact]
    public async Task GetAsync_WithSingleCookie_ShouldReturnCookie()
    {
        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie("sessionId", "ei34jdh")
        };

        var jsRuntime = GetMockJSRuntime(cookies);
        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);

        var sessionIdCookie = await cookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);
    }
    [Fact]
    public async Task GetAsync_WithSingleEmptyValueCookie_ShouldReturnCookie()
    {
        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie("sessionId", "")
        };

        var jsRuntime = GetMockJSRuntime(cookies);
        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);

        var sessionIdCookie = await cookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);
    }
    [Fact]
    public async Task GetAsync_WithEmptyCookiesList_ShouldReturnNull()
    {
        List<Cookie> cookies = new List<Cookie>();

        var jsRuntime = GetMockJSRuntime(cookies);
        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);

        var sessionIdCookie = await cookieService.GetAsync("sessionId");
        Assert.Null(sessionIdCookie);
    }
    [Fact]
    public async Task GetAsync_WithNullCookiesList_ShouldReturnNull()
    {
        List<Cookie>? cookies = null;

        var jsRuntime = GetMockJSRuntime(cookies);
        JsInteropCookieService cookieService = new JsInteropCookieService(jsRuntime);

        var sessionIdCookie = await cookieService.GetAsync("sessionId");
        Assert.Null(sessionIdCookie);
    }

    private JsInteropCookieService CreateMockSetAsyncCookieService(IEnumerable<Cookie>? cookies)
    {
        Mock<IJSRuntime> mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(jsRuntime => jsRuntime.InvokeAsync<IJSVoidResult>("eval", It.IsAny<object[]>()))
            .Returns(new ValueTask<IJSVoidResult>());
        mockJSRuntime.Setup(jsRuntime => jsRuntime.InvokeAsync<string?>("eval", It.IsAny<object[]>()))
            .ReturnsAsync(ToJsCookieString(cookies));

        return new JsInteropCookieService(mockJSRuntime.Object);
    }

    [Fact]
    public async Task SetAsync_WithCookieOverload_ShouldReturnCookie()
    {
        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        JsInteropCookieService jsInteropCookieService = CreateMockSetAsyncCookieService(
            new List<Cookie> { cookie }
        );

        await jsInteropCookieService.SetAsync(cookie);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieSameSiteOverload_ShouldReturnCookie()
    {
        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        JsInteropCookieService jsInteropCookieService = CreateMockSetAsyncCookieService(
            new List<Cookie> { cookie }
        );

        await jsInteropCookieService.SetAsync(cookie, SameSiteMode.Strict);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieNameValueOverload_ShouldReturnCookie()
    {
        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        JsInteropCookieService jsInteropCookieService = CreateMockSetAsyncCookieService(
            new List<Cookie> { cookie }
        );

        await jsInteropCookieService.SetAsync(cookie.Name, cookie.Value);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieNameValueExpiresOverload_ShouldReturnCookie()
    {
        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        JsInteropCookieService jsInteropCookieService = CreateMockSetAsyncCookieService(
            new List<Cookie> { cookie }
        );

        await jsInteropCookieService.SetAsync(cookie.Name, cookie.Value);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieNameValueExpiresSameSiteOverload_ShouldReturnCookie()
    {
        Cookie cookie = new Cookie
        {
            Name = "sessionId",
            Value = "ei34jdh",
            Expires = DateTime.UtcNow.AddDays(1)
        };
        JsInteropCookieService jsInteropCookieService = CreateMockSetAsyncCookieService(
            new List<Cookie> { cookie }
        );

        await jsInteropCookieService.SetAsync(
            cookie.Name,
            cookie.Value,
            cookie.Expires,
            SameSiteMode.Strict
        );

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
}
