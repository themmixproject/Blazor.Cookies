# MMIX.Blazor.Cookies

ASP.NET-compatible cookie management for Blazor.

MMIX.Blazor.Cookies is a modern, .NET 8+ library for easy browser cookie management across all Blazor render modes. It is a highly modified fork of Bitzart's [Blazor.Cookies](https://github.com/BitzArt/Blazor.Cookies) project, optimized for better ASP.NET integration and full prerendering support.

- Supports all Blazor United (.NET 8+) render modes: Static SSR, Interactive Server, Interactive WebAssembly, and Interactive Auto
- Compatible with ASP.NET and Blazor Web App projects
- Simple, dependency-injection-friendly API: inject `ICookieService` and manage cookies anywhere

### Installation
Install the following package in your Blazor Server and/or Blazor Client project:
```
dotnet add package MMIX.Blazor.Cookies
```
Add this line to your Server and/or Client project `program.cs`:

```csharp
builder.Services.AddCookieService();
```

### Usage

Injecting the `ICookieService` in a class:
```csharp
using MMIX.Blazor.Cookies;

public class Foo
{
    private readonly ICookieService _cookieService;

    public Foo(ICookieService cookieService)
    {
        _cookieService = cookieService;
    }

    public async Task SetCookie()
    {
        await _cookieService.SetAsync("hello", "world");
    }
}
```

Injecting the `ICookieService` in a Blazor component:
```csharp
@using MMIX.Blazor.Cookies

@inject ICookieService CookieService

@code {

    public async Task SetCookie() {
        await CookieService.SetAsync("hello", "world");
    }

}
```
### Supported Methods

The `ICookieService` interface provides the following methods:

#### Reading Cookies
```csharp
// Retrieves a cookei by its name
Task<Cookie?> GetAsync(string name);

// Retrieves all cookies
Task<IEnumerable<Cookie>> GetAllAsync();
```

#### Setting Cookies
```csharp
// Sets a cookie
Task SetAsync(string name, string value, CancellationToken cancellationToken = default);

// Sets a cookie with an expriation date
Task SetAsync(string name, string value, DateTime expires, CancellationToken cancellationToken = default);

// Sets a cookie with a CookieOptions object
Task SetAsync(string name, string value, CookieOptions cookieOptions, CancellationToken cancellationToken = default);

// Sets a cookie using a Cookie object
Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default);
```

#### Removing Cookies
```csharp
// Removes a cookie by its name
Task RemoveAsync(string name, CancellationToken cancellationToken = default);
```

## License

[![License](https://img.shields.io/badge/mit-%230072C6?style=for-the-badge)](https://github.com/themmixproject/MMIX.Blazor.Cookies/blob/main/LICENSE)

<br /><br />

### Credits to the original developers
- [Yui Durov](https://github.com/YuriyDurov "Yui Durov")
- [Vladimir Seldemirov](https://github.com/ligowsky "Vladimir Seldemirov")
- [Gaël James](https://github.com/gaelj)
