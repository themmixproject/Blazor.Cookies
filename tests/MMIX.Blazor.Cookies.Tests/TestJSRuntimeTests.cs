using System;
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
        string command = $"myCookie=myValue; expires={cookieExpires}; samesite=lax;";
        string cookieString = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");

        Assert.Equal(command, cookieString);
    }


}
