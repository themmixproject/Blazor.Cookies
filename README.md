## Overview

**MMIX.Blazor.Cookies** is a heavily modified version of **Bitzart's Blazor.Cookies** package with more emphasis on compability with AspDotnet. MMIX.Blazor.Cookies is a nuget package that makes working with cookies in Blazor easier. 

**Credits to the original developers**
- [Yui Durov](https://github.com/YuriyDurov "Yui Durov")
- [Vladimir Seldemirov](https://github.com/ligowsky "Vladimir Seldemirov")
- [Gaël James](https://github.com/gaelj)

------------

- Built for dotnet 8+
- Supports all Blazor United render modes
- Supports Blazor prerendering

| Blazor Rendermode       | Support |
|-------------------------|:-------:|
| Static SSR              | ✔      |
| Interactive Server      | ✔      |
| Interactive WebAssembly | ✔      |
| Interactive Auto        | ✔      |

<!-- ### Installation

- Install the following package in your Blazor Server project:

```
dotnet add package BitzArt.Blazor.Cookies.Server
```

- Add this line to your Server project `program.cs`:

```csharp
builder.AddBlazorCookies();
```

- Install the following package in your Blazor Client project:

```
dotnet add package BitzArt.Blazor.Cookies.Client
```

- Add this line to your Client project `program.cs`:

```csharp
builder.AddBlazorCookies();
``` -->

### Usage

- Inject `ICookieService` in any of your Services / Blazor Components
- Use `ICookieService` to interact with user's cookies.

## License

[![License](https://img.shields.io/badge/mit-%230072C6?style=for-the-badge)](https://github.com/themmixproject/Blazor.Cookies/blob/main/LICENSE)
