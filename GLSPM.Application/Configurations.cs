using FluentValidation.AspNetCore;
using GLSPM.Application.EFCore;
using GLSPM.Application.EFCore.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CubesFramework.Security;
using System.Security.Cryptography;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.AppServices;
using GLSPM.Domain;

namespace GLSPM.Application
{
    public static class Configurations
    {
        public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.ConfigureDB(configuration)
                    .ConfigEFCoreLayer()
                    .ConfigureFV()
                    .ComfigureCubesFW()
                    .AddHttpContextAccessor()
                    .ConfigureOptions<FilesPathes>()
                    .ConfigureAppSerivces();
            return services;
        }

        public static IServiceCollection ConfigureFV(this IServiceCollection services)
        {
            services.AddControllers()
               .AddFluentValidation(config =>
               {
                   config.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                   config.RegisterValidatorsFromAssembly(Assembly.GetCallingAssembly());
               });
            return services;
        }
        public static IServiceCollection ComfigureCubesFW(this IServiceCollection services)
        {
            var crypto = new Crypto(SHA256.Create());
            services.AddSingleton(crypto);
            return services;
        }
        public static IServiceCollection ConfigureDB(this IServiceCollection services, IConfiguration configuration)
        {
            var MSCS = configuration.GetConnectionString("MSCS");
            var MYSCS = configuration.GetConnectionString("MYSCS");
            var liteCS = configuration.GetConnectionString("liteCS");
            services.AddDbContext<GLSPMDBContext>(options =>
            {
                options.UseSqlServer(MSCS, c => c.MigrationsAssembly("GLSPM.Server"));
                options.EnableDetailedErrors();
            });
            return services;
        }
        public static IServiceCollection ConfigureAppSerivces(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppService<,,,,>), typeof(AppServiceBase<,,,,>));
            services.AddScoped<ICardsAppService,CardAppSerivce>();
            return services;
        }
    }
}
