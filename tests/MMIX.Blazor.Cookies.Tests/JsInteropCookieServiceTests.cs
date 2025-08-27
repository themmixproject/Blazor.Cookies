using MMIX.Blazor.Cookies.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using System.Net;
using MMIX.Blazor.Cookies.Tests.Patches;

namespace MMIX.Blazor.Cookies.Tests;
public class JsInteropCookieServiceTests
{
    [Fact]
    public async Task GetAllAsync_WithCookies_ShouldReturnCookieIEnumerable()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JsInteropCookieService(jsRuntime);

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

        var resultCookies = await jsInteropCookieService.GetAllAsync();
        Assert.NotEmpty(resultCookies);
        Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
        Assert.Equal(cookies, resultCookies);
    }
    [Fact]
    public async Task GetAllAsync_WithCookie_ShouldReturnCookieIEnumerable()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jSRuntime);

        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        await jsInteropCookieService.SetAsync(cookie);

        var resultCookies = await jsInteropCookieService.GetAllAsync();        
        Assert.NotEmpty(resultCookies);
        Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
        Assert.Contains(cookie, resultCookies);
    }
    [Fact]
    public async Task GetAllAsync_WithEmptyCookie_ShouldReturnCookieIEnumerable()
    {
        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie("sessionId", "")
        };

        IJSRuntime jSRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jSRuntime);
        foreach(Cookie cookie in cookies) { await jsInteropCookieService.SetAsync(cookie); }

        var resultCookies = await jsInteropCookieService.GetAllAsync();
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

        IJSRuntime jSRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jSRuntime);
        foreach(Cookie cookie in cookies) { await jsInteropCookieService.SetAsync(cookie); }

        var resultCookies = await jsInteropCookieService.GetAllAsync();
        Assert.NotEmpty(resultCookies);
        Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
        Assert.Equal(cookies, resultCookies);
    }
    [Fact]
    public async Task GetAllAsync_WithEmptyCookies__ShouldReturnCookieIEnumerable()
    {
        List<Cookie> cookies = new List<Cookie>();

        IJSRuntime jSRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jSRuntime);

        var resultCookies = await jsInteropCookieService.GetAllAsync();
        Assert.Empty(resultCookies);
        Assert.IsAssignableFrom<IEnumerable<Cookie>>(resultCookies);
        Assert.Equal(cookies, resultCookies);
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

        IJSRuntime jsRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jsRuntime);
        foreach(Cookie cookie in cookies) { await jsInteropCookieService.SetAsync(cookie); }

        var sessionIdCookie = await jsInteropCookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);

        var userIdCookie = await jsInteropCookieService.GetAsync("userId");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "userId"), userIdCookie);

        var themeCookie = await jsInteropCookieService.GetAsync("theme");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "theme"), themeCookie);

        var cartItemsCookie = await jsInteropCookieService.GetAsync("cartItems");
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

        var jsRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jsRuntime);
        foreach(Cookie cookie in cookies) { await jsInteropCookieService.SetAsync(cookie); }

        var sessionIdCookie = await jsInteropCookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);

        var userIdCookie = await jsInteropCookieService.GetAsync("userId");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "userId"), userIdCookie);

        var themeCookie = await jsInteropCookieService.GetAsync("theme");
        Assert.NotNull(userIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "theme"), themeCookie);

        var cartItemsCookie = await jsInteropCookieService.GetAsync("cartItems");
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

        var jsRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jsRuntime);
        foreach(Cookie cookie in cookies) { await jsInteropCookieService.SetAsync(cookie); }

        var sessionIdCookie = await jsInteropCookieService.GetAsync("sessionId");
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

        var jsRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jsRuntime);
        foreach(Cookie cookie in cookies) { await jsInteropCookieService.SetAsync(cookie); }

        var sessionIdCookie = await jsInteropCookieService.GetAsync("sessionId");
        Assert.NotNull(sessionIdCookie);
        Assert.Equal(cookies.First(c => c.Name == "sessionId"), sessionIdCookie);
    }
    [Fact]
    public async Task GetAsync_WithEmptyCookiesList_ShouldReturnNull()
    {
        var jsRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jsRuntime);

        var sessionIdCookie = await jsInteropCookieService.GetAsync("sessionId");
        Assert.Null(sessionIdCookie);
    }

    [Fact]
    public async Task SetAsync_WithCookieOverload_ShouldReturnCookie()
    {
        var cookie = new Cookie("sessionId", "ei34jdh");

        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JsInteropCookieService(jsRuntime);
        await jsInteropCookieService.SetAsync(cookie);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieSameSiteOverload_ShouldReturnCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JsInteropCookieService(jsRuntime);

        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        await jsInteropCookieService.SetAsync(cookie, SameSiteMode.Strict);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieNameValueOverload_ShouldReturnCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JsInteropCookieService(jsRuntime);

        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        await jsInteropCookieService.SetAsync(cookie.Name, cookie.Value);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(cookie);
        Assert.Equal(cookie, resultCookie);
    }
    [Fact]
    public async Task SetAsync_WithCookieNameValueExpiresOverload_ShouldReturnCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JsInteropCookieService(jsRuntime);

        Cookie cookie = new Cookie("sessionId", "ei34jdh");
        cookie.Expires = DateTime.UtcNow.AddHours(1);

        await jsInteropCookieService.SetAsync(cookie.Name, cookie.Value, cookie.Expires);

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
        
        IJSRuntime jSRuntime = new VirtualJSRuntime();
        JsInteropCookieService jsInteropCookieService = new JsInteropCookieService(jSRuntime);

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
    [Fact]
    public async Task SetAsync_NameValueCookieOptionsOverload_ShouldReturnCookie()
    {
        var jsRuntime = new VirtualJSRuntime();
        var jsInteropCookieService = new JsInteropCookieService(jsRuntime);

        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        CookieOptions options = new CookieOptions
        {
            Expires = cookie.Expires,
            SameSite = SameSiteMode.Strict
        };

        await jsInteropCookieService.SetAsync(cookie.Name, cookie.Value, options);

        var resultCookie = await jsInteropCookieService.GetAsync(cookie.Name);
        Assert.NotNull(resultCookie);
        Assert.Equal(cookie, resultCookie);
    }
}
