using MMIX.Blazor.Cookies.Server;
using MMIX.Blazor.Cookies.Tests.Patches;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace MMIX.Blazor.Cookies.Tests;
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
        (var httpContext, var cookieService) = CreateTestDependencies();

        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        List<Cookie> cookies = new List<Cookie>
        {
            new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire },
            new Cookie { Name = "userId", Value = "xyz789", Expires = cookieExpire },
            new Cookie { Name = "theme", Value = "dark", Expires = cookieExpire },
            new Cookie { Name = "cartItems", Value = "5", Expires = cookieExpire }
        };

        foreach (Cookie cookie in cookies)
        {
            await cookieService.SetAsync(cookie);
        }

        var resultCookiesValues = httpContext.Response.Headers[HeaderNames.SetCookie];
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

        var resultCookiesValues = httpContext.Response.Headers[HeaderNames.SetCookie];
        string resultCookies = resultCookiesValues.ToString();
        Assert.NotEmpty(resultCookies);
        for (int i = 0; i < cookies.Count; i++)
        {
            Assert.Contains($"{cookies[i].Name}={cookies[i].Value}", resultCookies);
        }
    }

    [Fact]
    public async Task GetAllAsync_WithNoCookiesSet_ShouldReturnEmptyIEnumerable()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();

        var cookies = await cookieService.GetAllAsync();

        Assert.Empty(cookies);
    }

    [Fact]
    public async Task SetAsync_WithCookieObject_ShoudSetResponseHeaderCookie()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();

        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        await cookieService.SetAsync(cookie);

        var responseCookie = httpContext.Response.Headers[HeaderNames.SetCookie][0]!;
        var cookieString = $"{cookie.Name}={cookie.Value}; expires={cookie.Expires:R}; path=/";
        Assert.NotEmpty(responseCookie);
        Assert.Contains(cookieString, responseCookie);
    }

    [Fact]
    public async Task SetAsync_WithCookieSameSite_ShoudSetResponseHeaderCookie()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();

        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        await cookieService.SetAsync(cookie, SameSiteMode.Strict);

        var responseCookie = httpContext.Response.Headers[HeaderNames.SetCookie][0]!;
        var cookieString = $"{cookie.Name}={cookie.Value}; expires={cookie.Expires:R}; path=/; samesite=strict";
        Assert.NotEmpty(responseCookie);
        Assert.Contains(cookieString, responseCookie);
    }

    [Fact]
    public async Task SetAsync_WithNameValue_ShouldSetResponseHeaderCookie()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();

        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        await cookieService.SetAsync(cookie.Name, cookie.Value);

        var responseCookie = httpContext.Response.Headers[HeaderNames.SetCookie][0]!;
        var cookieString = $"{cookie.Name}={cookie.Value}";
        Assert.NotEmpty(responseCookie);
        Assert.Contains(cookieString, responseCookie);
    }

    [Fact]
    public async Task SetAsync_WithNameValueExpires_ShoudSetResponseHeaderCookie()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();

        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        await cookieService.SetAsync(cookie.Name, cookie.Value, cookie.Expires);

        var responseCookie = httpContext.Response.Headers[HeaderNames.SetCookie][0]!;
        var cookieString = $"{cookie.Name}={cookie.Value}; expires={cookie.Expires:R}";
        Assert.NotEmpty(responseCookie);
        Assert.Contains(cookieString, responseCookie);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("=;")]
    [InlineData("")]
    public async Task SetAsync_WithNameValue_WithInvalidName_ShouldThrowException(string invalidCookieName)
    {
        (var httpContext, var cookieService) = CreateTestDependencies();

        await Assert.ThrowsAsync<CookieException>(() =>
            cookieService.SetAsync(invalidCookieName, "cookieValue")
        );
    }

    [Fact]
    public async Task SetAsync_WithNameValueExpiresSameSite_ShoudSetResponseHeaderCookie()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();
        DateTime cookieExpire = DateTime.UtcNow.AddDays(1);
        SameSiteMode cookieSameSiteMode = SameSiteMode.Strict;
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        await cookieService.SetAsync(cookie.Name, cookie.Value, cookie.Expires, cookieSameSiteMode);

        var responseCookie = httpContext.Response.Headers[HeaderNames.SetCookie][0]!;
        var sameSiteStringValue = cookieSameSiteMode.ToString().ToLower();
        var cookieString = $"{cookie.Name}={cookie.Value}; expires={cookie.Expires:R}; path=/; samesite={sameSiteStringValue}";
        Assert.NotEmpty(responseCookie);
        Assert.Contains(cookieString, responseCookie);
    }

    [Fact]
    public async Task SetAsync_NameValueCookieOptionsOverload_ShouldReturnCookie()
    {
        (var httpContext, var cookieService) = CreateTestDependencies();
        DateTime cookieExpire = DateTime.UtcNow.AddDays( 1 );
        Cookie cookie = new Cookie { Name = "sessionId", Value = "ei34jdh", Expires = cookieExpire };
        CookieOptions options = new CookieOptions
        {
            Expires = cookie.Expires,
            SameSite = SameSiteMode.Strict
        };

        await cookieService.SetAsync( cookie.Name, cookie.Value, options );

        var responseCookie = httpContext.Response.Headers[HeaderNames.SetCookie][0]!;
        Assert.NotEmpty(responseCookie);
        Assert.Contains($"{cookie.Name}={cookie.Value}", responseCookie);
        Assert.Contains( $"samesite={options.SameSite.ToString().ToLowerInvariant()}", responseCookie );
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveCookie()
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

        foreach (Cookie cookie in cookies)
        {
            await cookieService.SetAsync(cookie);
        }

        for (int i = 0; i < cookies.Count; i++)
        {
            var cookie = cookies[i];

            await cookieService.RemoveAsync(cookie.Name);

            var responseCookies = httpContext.Response.Headers[HeaderNames.SetCookie];
            Assert.DoesNotContain<string>($"{cookie.Name}={cookie.Value}", responseCookies);
        }

        Assert.Equal(0, httpContext.Response.Headers[HeaderNames.SetCookie].Count);
    }
}
