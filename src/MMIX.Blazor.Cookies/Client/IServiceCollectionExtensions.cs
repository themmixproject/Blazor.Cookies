using Microsoft.Extensions.DependencyInjection;

namespace MMIX.Blazor.Cookies.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCookieService(this IServiceCollection services)
        {
            services.AddScoped<ICookieService, JSInteropCookieService>();
            return services;
        }

        public static IServiceCollection AddCookieService(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(ICookieService),
                    typeof(JSInteropCookieService),
                    lifetime
                )
            );

            return services;
        }
    }
}
