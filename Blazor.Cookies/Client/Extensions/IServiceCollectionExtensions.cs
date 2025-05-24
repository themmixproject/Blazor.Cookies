using Blazor.Cookies.Client.Services;
using Blazor.Cookies.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
