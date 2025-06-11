using Blazor.Cookies.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Blazor.Cookies.SampleApp.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddCookieService();

            await builder.Build().RunAsync();
        }
    }
}
