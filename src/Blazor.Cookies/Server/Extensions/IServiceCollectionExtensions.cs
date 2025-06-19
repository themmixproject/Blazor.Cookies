using Blazor.Cookies.Client.Services;
using Blazor.Cookies.Interfaces;
using Blazor.Cookies.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Cookies.Server.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCookieService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<JsInteropCookieService>();
        services.AddScoped<HttpContextCookieService>();
        services.AddScoped<ICookieService>(i =>
        {
            var httpContextAccessor = i.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var isPrerendering = (httpContext != null && !httpContext.Response.HasStarted);

            if (isPrerendering) { return i.GetRequiredService<HttpContextCookieService>(); }
            else { return i.GetRequiredService<JsInteropCookieService>(); }
        });

        return services;
    }
}
