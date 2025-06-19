using MMIX.Blazor.Cookies.Client.Services;
using MMIX.Blazor.Cookies.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MMIX.Blazor.Cookies.Client.Extensions
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
