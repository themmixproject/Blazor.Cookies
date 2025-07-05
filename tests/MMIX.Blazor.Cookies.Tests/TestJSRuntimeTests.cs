using System;
using System.Reflection.Metadata;
using Microsoft.JSInterop;
using MMIX.Blazor.Cookies.Tests.Patches;

namespace MMIX.Blazor.Cookies.Tests;

public class TestJSRuntimeTests
{
    private const string cookieDateFormat = "ddd, dd yyyy hh:mm:ss \'UTC\'zzz";

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_WithNoCookies_ShouldReturnEmptyString()
    {
        IJSRuntime jsRuntime = new TestJSRuntime();
        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");

        Assert.Empty(cookies);
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_WithSetCookieCommand_ShouldReturnCookieString()
    {
        IJSRuntime jsRuntime = new TestJSRuntime();

        string cookieExpires = DateTime.Today.ToUniversalTime().ToString(cookieDateFormat);
        string cookieString = $"myCookie=myValue; expires={cookieExpires}; samesite=lax;";
        string command = $"document.cookie = '{cookieString}'";
        string ouputCookieString = await jsRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, ouputCookieString);
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_WithOnlyNameValueWithoutSemicolon_ShouldReturnCookieString()
    {
        IJSRuntime jSRuntime = new TestJSRuntime();

        string cookieString = "myCookie=myValue";
        string command = $"document.cookie = '{cookieString}'";
        string outputCookieString = await jSRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_WithOnlyNameValueWithSemicolon_ShouldReturnCookieString()
    {
        IJSRuntime jSRuntime = new TestJSRuntime();

        string cookieString = "myCookie=myValue;";
        string command = $"document.cookie = 'myCookie=myValue;'";
        string outputCookieString = await jSRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_SetCookieWithOnlyNameValueAttributesWithoutSemicolon_ShouldSetCookieWithoutTrailingSemicolon()
    {
        IJSRuntime jSRuntime = new TestJSRuntime();

        string cookieNameValue = $"myCookie=myValue";
        string command = $"document.cookie = '{cookieNameValue}'";
        await jSRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookies, $"{cookieNameValue}");
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_SetCookieWithOtherProperties_ShouldSetCookieWithoutTrailingSemicolon()
    {
        IJSRuntime jSRuntime = new TestJSRuntime();

        string cookieNameValue = "myCookie=myValue";
        string cookieExpires = DateTime.Today.ToUniversalTime().ToString(cookieDateFormat);
        string command = $"document.cookie = '{cookieNameValue}; expires={cookieExpires}; samesite=lax;'";
        await jSRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookies, $"{cookieNameValue}");
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_SetCookieWithMixedOtherProperties_ShouldSetCookieWithoutTrailingSemicolon()
    {
        IJSRuntime jSRuntime = new TestJSRuntime();

        string cookieNameValue = "myCookie=myValue";
        string cookieExpires = DateTime.Today.ToUniversalTime().ToString(cookieDateFormat);
        string command = $"document.cookie = 'samesite=lax; path=/; expires={cookieExpires}; {cookieNameValue}'";
        await jSRuntime.InvokeAsync<string>("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookies, $"{cookieNameValue}");
    }
}
