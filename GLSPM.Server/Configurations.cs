using GLSPM.Application;
using GLSPM.Application.EFCore;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace GLSPM.Server
{
    public static class Configurations
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.AddSerilog();
            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
                                options.SerializerSettings.ReferenceLoopHandling
                                = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            builder.Services.AddRazorPages();

            builder.Services.ConfigureApplicationLayer(builder.Configuration, builder.Environment);

            builder.Services.AddSwaggerGen(config =>
            {
                string description = " Password Manager WEB apis";
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "GLSPM", Version = "v1", Description = description });
            });
            return builder;
        }
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
            return builder;
        }

        public static UserManager<ApplicationUser> SeedDefUsers(this UserManager<ApplicationUser> userManager)
        {
            var admin = new ApplicationUser
            {
                Id = "1",
                UserName = "Admin",
                NormalizedUserName = "Admin".ToUpper(),
                Email = "Admin@GoodLawSoftware.com",
                NormalizedEmail = "Admin@GoodLawSoftware.com".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "201120797422",
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
            };
            var password = "Admin@2022";
            if (userManager.FindByIdAsync("1").Result==null)
            {
                if (userManager.CreateAsync(admin, password).Result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
            }

            return userManager;
        }
    }
}
