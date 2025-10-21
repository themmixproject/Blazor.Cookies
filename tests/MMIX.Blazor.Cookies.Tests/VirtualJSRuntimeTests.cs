using System.Globalization;
using Microsoft.JSInterop;
using MMIX.Blazor.Cookies.Patches.JSInterop;

namespace MMIX.Blazor.Cookies.Tests;

public class VirtualJSRuntimeTests
{
    private const string cookieDateFormat = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

    [Fact]
    public async Task DocumentCookie_ShouldReturnEmptyString()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();
        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");

        Assert.Empty(cookies);
    }

    [Fact]
    public async Task SetCookie_ShouldReturnCookieString()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieExpires = DateTime.Today.ToUniversalTime().ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        string cookieString = $"myCookie=myValue; expires={cookieExpires}; samesite=lax;";
        string command = $"document.cookie = '{cookieString}'";
        string outputCookieString = await jsRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }

    [Fact]
    public async Task SetCookieWithoutSemicolonSuffix_ShouldReturnCookieString()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieString = "myCookie=myValue";
        string command = $"document.cookie = '{cookieString}'";
        string outputCookieString = await jSRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }

    [Fact]
    public async Task SetCookieWithSemicolonSuffix_ShouldReturnCookieString()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieString = "myCookie=myValue;";
        string command = $"document.cookie = 'myCookie=myValue;'";
        string outputCookieString = await jSRuntime.InvokeAsync<string>("eval", command);

        Assert.Equal(cookieString, outputCookieString);
    }

    [Fact]
    public async Task SetCookieWithProperties_ShouldSetCookieWithoutTrailingSemicolon()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieNameValue = "myCookie=myValue";
        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        string command = $"document.cookie = '{cookieNameValue}; expires={cookieExpires}; samesite=lax;'";
        await jSRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookieNameValue, cookies);
    }

    [Fact]
    public async Task SetCookieWithMixedProperties_ShouldSetCookie()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieNameValue = "myCookie=myValue";
        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        string command = $"document.cookie = 'samesite=lax; path=/; expires={cookieExpires}; {cookieNameValue}'";
        await jSRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookieNameValue, cookies);
    }

    [Fact]
    public async Task SetCookieWithEmptyName_ShouldSetCookieValue()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieValue = "myValue";
        string command = $"document.cookie = '{cookieValue}'";
        await jSRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookieValue, cookies);
    }
    
    [Fact]
    public async Task SetCookieWithEqualsAfterValue_ShouldSetCookieValue()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieValue = "myValue=";
        string command = $"document.cookie = '{cookieValue}'";
        await jSRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookieValue, cookies);
    }

    [Fact]
    public async Task SetCookieWithEmptyNameAndSetCookieWithEmptyValue_ShouldSetCookies()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();
        string emptyNameCookie = "myValue";
        string emptyValueCookie = "myCookie=";

        string emptyNameCommand = $"document.cookie = '{emptyNameCookie}'";
        await jSRuntime.InvokeVoidAsync("eval", emptyNameCommand);
        string emptyValueCommand = $"document.cookie = '{emptyValueCookie}'";
        await jSRuntime.InvokeVoidAsync("eval", emptyValueCommand);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(emptyNameCookie, cookies);
        Assert.Contains(emptyValueCookie, cookies);
    }

    [Fact]
    public async Task SetTwoCookiesWithOnlyValue_ShouldOverWriteFirstCookie()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string value_1 = "myValue_1";
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{value_1}'");
        string value_2 = "myValue_2";
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{value_2}'");

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.DoesNotContain(value_1, cookies);
        Assert.Contains(value_2, cookies);
    }

    [Fact]
    public async Task SetMultipleCookies_ShouldSetCookies()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        await jsRuntime.InvokeVoidAsync("eval", "document.cookie = 'cookie1=value1'");
        await jsRuntime.InvokeVoidAsync("eval", "document.cookie = 'cookie2=value2'");
        await jsRuntime.InvokeVoidAsync("eval", "document.cookie = 'cookie3=value3'");

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains("cookie1=value1", cookies);
        Assert.Contains("cookie2=value2", cookies);
        Assert.Contains("cookie3=value3", cookies);
    }

    [Fact]
    public async Task SetCookieWithSpecialCharacters_ShouldSetCookie()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieString = "special=value with spaces & symbols!@#$%";
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieString}'");

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains(cookieString, cookies);
    }

    [Fact]
    public async Task SetCookieWithEmptyName_ShouldSetCookie()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        await jsRuntime.InvokeVoidAsync("eval", "document.cookie = '=emptyNameValue'");

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains("emptyNameValue", cookies);
    }

    [Fact]
    public async Task SetCookieWithCaseInsensitiveAttributes_ShouldSetCookie()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'test=value; EXPIRES={cookieExpires}; PATH=/; SECURE'");

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains("test=value", cookies);
    }

    [Fact]
    public async Task SetCookieWithDuplicateAttributes_ShouldUseFirstOccurrence()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        await jsRuntime.InvokeVoidAsync("eval", "document.cookie = 'test=value; path=/first; path=/second'");

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains("test=value", cookies);
    }

    [Fact]
    public async Task SetCookieWithDoubleQuotes_ShouldSetCookie()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieString = "testCookie=testValue";
        string command = $"document.cookie = \"{cookieString}\"";
        await jsRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookieString, cookies);
    }

    [Fact]
    public async Task SetCookieWithSingleQuotes_ShouldSetCookie()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieString = "testCookie=testValue";
        string command = $"document.cookie = '{cookieString}'";
        await jsRuntime.InvokeVoidAsync("eval", command);

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.NotEmpty(cookies);
        Assert.Contains(cookieString, cookies);
    }

    [Fact]
    public async Task RemoveCookieWithKey_ShouldRemoveCookie()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieName = "myCookie";
        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName} = myValue; expires={cookieExpires}'");

        cookieExpires = DateTime.UtcNow.AddHours(-1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName} = myValue; expires={cookieExpires}'");

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookies);
    }
    [Fact]
    public async Task RemoveCookieWithKeyDuringTest_ShouldRemoveCookie()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieName = "myCookie";
        string cookieExpires = DateTime.UtcNow.AddSeconds(2).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{  cookieName} = myvalue; expires={cookieExpires}'");

        Thread.Sleep(3000);

        string cookies = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookies);
    }

    [Fact]
    public async Task RemoveCookieWithoutKey_ShouldRemoveCookie()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieName = "myCookie";
        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName}; expires={cookieExpires}'");

        cookieExpires = DateTime.UtcNow.AddHours(-1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName}; expires={cookieExpires}'");

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookies);
    }

    [Fact]
    public async Task RemoveCookieWithoutKeyDuringTest_ShouldRemoveCookie()
    {
        IJSRuntime jSRuntime = new VirtualJSRuntime();

        string cookieName = "myCookie";
        string cookieExpires = DateTime.UtcNow.AddSeconds(2).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jSRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName}; expires={cookieExpires}'");

        Thread.Sleep(3000);

        string cookies = await jSRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookies);
    }

    [Fact]
    public async Task RemoveMultipleCookiesByExpiration_ShouldRemoveAllCookies()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie1=value1; expires={cookieExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie2=value2; expires={cookieExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie3=value3; expires={cookieExpires}'");

        string pastExpires = DateTime.UtcNow.AddHours(-1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie1=value1; expires={pastExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie2=value2; expires={pastExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie3=value3; expires={pastExpires}'");

        string cookiesAfterRemoval = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookiesAfterRemoval);
    }

    [Fact]
    public async Task RemoveSomeOfMultipleCookies_ShouldRemoveSelectedCookies()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie1=value1; expires={cookieExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie2=value2; expires={cookieExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie3=value3; expires={cookieExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie4=value4; expires={cookieExpires}'");

        string pastExpires = DateTime.UtcNow.AddHours(-1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie1=value1; expires={pastExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'cookie3=value3; expires={pastExpires}'");

        string cookiesAfterRemoval = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.DoesNotContain("cookie1=value1", cookiesAfterRemoval);
        Assert.Contains("cookie2=value2", cookiesAfterRemoval);
        Assert.DoesNotContain("cookie3=value3", cookiesAfterRemoval);
        Assert.Contains("cookie4=value4", cookiesAfterRemoval);
    }

    [Fact]
    public async Task RemoveMultipleCookiesByTimeout_ShouldRemoveExpiredCookies()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string shortExpires = DateTime.UtcNow.AddSeconds(2).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        string longExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);

        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'shortCookie1=value1; expires={shortExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'shortCookie2=value2; expires={shortExpires}'");
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'longCookie=value3; expires={longExpires}'");

        Thread.Sleep(3000);

        string cookiesAfterTimeout = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.DoesNotContain("shortCookie1=value1", cookiesAfterTimeout);
        Assert.DoesNotContain("shortCookie2=value2", cookiesAfterTimeout);
        Assert.Contains("longCookie=value3", cookiesAfterTimeout);
    }
    
    [Fact]
    public async Task CreateAndRemoveSingleCookieByExpiration_ShouldCreateThenRemove()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();
        
        string cookieName = "testCookie";
        string cookieValue = "testValue";
        string futureExpires = DateTime.UtcNow.AddHours(1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName}={cookieValue}; expires={futureExpires}'");
        
        string cookiesAfterCreation = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains($"{cookieName}={cookieValue}", cookiesAfterCreation);
        
        string pastExpires = DateTime.UtcNow.AddHours(-1).ToString(cookieDateFormat, CultureInfo.InvariantCulture);
        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName}={cookieValue}; expires={pastExpires}'");
        
        string cookiesAfterRemoval = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookiesAfterRemoval);
    }

    [Fact]
    public async Task CreateAndRemoveSingleCookieByTimeout_ShouldCreateThenRemove()
    {
        IJSRuntime jsRuntime = new VirtualJSRuntime();

        string cookieName = "timeoutCookie";
        string cookieValue = "timeoutValue";
        string shortExpires = DateTime.UtcNow.AddSeconds(2).ToString(cookieDateFormat, CultureInfo.InvariantCulture);

        await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieName}={cookieValue}; expires={shortExpires}'");

        string cookiesAfterCreation = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Contains($"{cookieName}={cookieValue}", cookiesAfterCreation);

        Thread.Sleep(3000);

        string cookiesAfterTimeout = await jsRuntime.InvokeAsync<string>("eval", "document.cookie");
        Assert.Empty(cookiesAfterTimeout);
    }
}
