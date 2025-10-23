using System.Net;
using Microsoft.AspNetCore.Http;
using MMIX.Blazor.Cookies.Client;
using MMIX.Blazor.Cookies.Patches.JSInterop;

namespace MMIX.Blazor.Cookies.Tests;

public class JsInteropCookieServiceTests
{
    [Fact]
    public async Task GetAllAsync_WithCookiesSet_ShouldReturnCookies()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = await new JSInteropCookieService(jsRuntime).InitializeAsync();

        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie("sessionId", "ei34jdh"),
            new Cookie("userId", "xyz789"),
            new Cookie("theme", "dark"),
            new Cookie("cartItems", "5")
        };
        foreach (Cookie cookie in cookies)
        {
            await jsInteropCookieService.SetAsync(cookie);
        }

        var returnCookies = await jsInteropCookieService.GetAllAsync();

        Assert.Equal(cookies.Count, returnCookies.Count());

        foreach (Cookie cookie in returnCookies)
        {
            Assert.Contains(cookie, cookies);
        }
    }

    [Fact]
    public async Task GetAllAsync_WithNoCookiesSet_ShouldReturnEmpty()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        var returnCookies = await jsInteropCookieService.GetAllAsync();

        Assert.Empty(returnCookies);
    }

    [Fact]
    public async Task GetAsync_WithCookieSet_ShouldReturnCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        var cookie = new Cookie("myCookie", "myValue");
        await jsInteropCookieService.SetAsync(cookie);

        var returnCookie = await jsInteropCookieService.GetAsync("myCookie");
        Assert.NotNull(returnCookie);
        Assert.IsType<Cookie>(returnCookie);
    }

    [Theory]
    [InlineData("myCookie")]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetAsync_WithNonExistentCookie_ShouldReturnNull(string cookieName)
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        Cookie? cookie = await jsInteropCookieService.GetAsync(cookieName);
        Assert.Null(cookie);
    }

    [Fact]
    public async Task SetAsync_WithCookieObject_ShouldSetCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        Cookie cookie = new Cookie("myCookie", "myValue");
        await jsInteropCookieService.SetAsync(cookie);

        Cookie? returnCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.Equal(cookie, returnCookie);
    }

    [Theory]
    [InlineData("myCookie", "cookieValue")]
    [InlineData("myCookie", "")]
    public async Task SetAsync_WithNameValue_ShouldSetCookie(string cookieName, string cookieValue)
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        Cookie cookie = new Cookie(cookieName, cookieValue);
        await jsInteropCookieService.SetAsync(cookie.Name, cookie.Value);

        Cookie? returnCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.Equal(cookie, returnCookie);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("=;")]
    [InlineData("")]
    public async Task SetAsync_WithInvalidName_ShouldThrowCookieException(string invalidCookieName)
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        await Assert.ThrowsAsync<CookieException>(() =>
            jsInteropCookieService.SetAsync(invalidCookieName, "cookieValue")
        );
    }

    [Fact]
    public async Task SetAsync_WithNameValueExpires_ShouldSetCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        string cookieName = "myCookie";
        string cookieValue = "myValue";
        DateTime cookieExpires = DateTime.UtcNow.AddHours(1);
        await jsInteropCookieService.SetAsync(cookieName, cookieValue, cookieExpires);

        Cookie? returnCookie = await jsInteropCookieService.GetAsync(cookieName);
        Assert.NotNull(returnCookie);
        Assert.Equal(returnCookie.Name, cookieName);
        Assert.Equal(returnCookie.Value, cookieValue);
    }

    [Fact]
    public async Task SetAsync_WithNameValueCookieOptions_ShouldSetCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        string cookieName = "myCookie";
        string cookieValue = "myValue";
        CookieOptions cookieOptions = new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddHours(1),
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };
        await jsInteropCookieService.SetAsync(cookieName, cookieValue, cookieOptions);

        Cookie? returnCookie = await jsInteropCookieService.GetAsync(cookieName);
        Assert.NotNull(returnCookie);
        Assert.Equal(returnCookie.Name, cookieName);
        Assert.Equal(returnCookie.Value, cookieValue);
    }

    [Fact]
    public async Task SetAsync_UpdateToPastExpires_ShouldRemoveCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        string cookieName = "myCookie";
        string cookieValue = "cookieValue";

        var newCookie = new Cookie
        {
            Name = cookieName,
            Value = cookieValue,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        await jsInteropCookieService.SetAsync(newCookie);

        var expiredCookie = new Cookie
        {
            Name = cookieName,
            Value = cookieValue,
            Expires = DateTime.UtcNow.AddHours(-1)
        };
        await jsInteropCookieService.SetAsync(expiredCookie);

        Cookie? resultCookie = await jsInteropCookieService.GetAsync(cookieName);
        Assert.Null(resultCookie);
    }

    [Fact]
    public async Task SetAsync_AfterCookieExpires_ShouldRemoveCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        string cookieName = "myCookie";
        string cookieValue = "cookieValue";

        var cookie = new Cookie
        {
            Name = cookieName,
            Value = cookieValue,
            Expires = DateTime.UtcNow.AddSeconds(1)
        };
        await jsInteropCookieService.SetAsync(cookie);

        await Task.Delay(2500);

        Cookie? resultCookie = await jsInteropCookieService.GetAsync(cookieName);
        Assert.Null(resultCookie);
    }

    [Fact]
    public async Task RemoveCookie_ShouldRemoveCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        var cookie = new Cookie
        {
            Name = "myCookie",
            Value = "cookieValue",
            Expires = DateTime.UtcNow.AddHours(1)
        };
        await jsInteropCookieService.SetAsync(cookie);

        await jsInteropCookieService.RemoveAsync(cookie.Name);

        Cookie? resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.Null(resultCookie);
    }

    [Fact]
    public async Task RemoveAllCookies_ShouldRemoveAllCookies()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JSInteropCookieService(jsRuntime);

        await jsInteropCookieService.SetAsync("myCookie_1", "myValue_1");
        await jsInteropCookieService.SetAsync("myCookie_2", "myValue_2");
        await jsInteropCookieService.SetAsync("myCookie_3", "myValue_3");

        await jsInteropCookieService.RemoveAllAsync();

        var cookies = await jsInteropCookieService.GetAllAsync();
        Assert.Empty(cookies);
    }
}
