using Blazored.LocalStorage;
using GLSPM.Client.Services;
using GLSPM.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace GLSPM.Client
{
    public static class Configurations
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services, IWebAssemblyHostEnvironment environment)
        {
            services
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(environment.BaseAddress) })
                .AddMudServices()
                .AddBlazoredLocalStorage()
                .AddScoped<IAccountsService, AccountsService>();
            return services;
        }
    }
}
