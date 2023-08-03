using crudBundle.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace crudBundle
{
    public static class ConfiguredServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            builder.Services.AddTransient<ResponseHeaderActionFilter>();

            builder.Services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5); // 5 is Order
                var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                options.Filters.Add(new ResponseHeaderActionFilter(logger) { Key = "X-Key-From-Global", Value = "-X-Key-From-Global", Order = 2 });
            });

            //add services into IoC Container
            builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
            builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();

            builder.Services.AddDbContext<ApplicationDbContext>
                (options =>
                {
                    options.UseSqlServer(builder.Configuration
                        .GetConnectionString("DefaultConnection"));
                });

            builder.Services.AddTransient<PersonsListActionFilter>();

            builder.Services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
        }
    }
}
