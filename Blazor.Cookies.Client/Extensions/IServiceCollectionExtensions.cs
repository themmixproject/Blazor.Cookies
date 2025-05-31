using Blazor.Cookies.Client.Services;
using Blazor.Cookies.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Cookies.Client.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCookieService(this IServiceCollection services)
        {
            services.AddScoped<ICookieService, JsInteropCookieService>();
            return services;
        }

        public static IServiceCollection AddCookieService(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(ICookieService),
                    typeof(JsInteropCookieService),
                    lifetime
                )
            );

            return services;
        }
    }
}
