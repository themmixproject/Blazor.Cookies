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
        string cookies = await jsRuntime.InvokeAsync<String>("eval", "document.cookie");

        Assert.Empty(cookies);
    }

    
}
