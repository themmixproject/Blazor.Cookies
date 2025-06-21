using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MMIX.Blazor.Cookies.Client;

namespace MMIX.Blazor.Cookies.Server;
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
            var isPrerendering = httpContext != null && !httpContext.Response.HasStarted;

            if (isPrerendering) { return i.GetRequiredService<HttpContextCookieService>(); }
            else { return i.GetRequiredService<JsInteropCookieService>(); }
        });

        return services;
    }
}
