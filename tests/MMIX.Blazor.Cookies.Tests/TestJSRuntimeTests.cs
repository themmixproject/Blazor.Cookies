using System;
using System.Reflection.Metadata;
using Microsoft.JSInterop;
using MMIX.Blazor.Cookies.Tests.Patches;

namespace MMIX.Blazor.Cookies.Tests;

public class TestJSRuntimeTests
{
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
        DateTime date = DateTime.Today;
        string cookieExpires = date.ToUniversalTime().ToString("ddd, dd yyyy hh:mm:ss \'UTC\'zzz");
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
        string command = $"document.cookie = '{cookieString}'";
        string outputCookieString = await jSRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }

    [Fact]
    public async Task InvokeAsync_eval_documentcookie_SetCookieWithOnlyNameValueAttributesWithoutSemicolon_ShouldSetCookie()
    {
        IJSRuntime jSRuntime = new TestJSRuntime();

        string cookieString = "myCookie=myValue;";
        string command = $"document.cookie = '{cookieString}'";
        string outputCookieString = await jSRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }


}
