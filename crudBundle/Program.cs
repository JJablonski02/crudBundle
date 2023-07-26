using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using crudBundle.Filters.ActionFilters;

var builder = WebApplication.CreateBuilder(args);


//logging
//builder.Host.ConfigureLogging(loggingProvider =>
//{
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//    loggingProvider.AddDebug();
//    loggingProvider.AddEventLog();
//});

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services); // Reading the configuration
                                                                                                   // from appsettings.json, reads out
                                                                                                   // current app's services and make them available to serilog
});

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

var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogCritical("critical-message");
//app.Logger.LogError("error-message");
//app.Logger.LogError("warning-message");

if (builder.Environment.IsEnvironment("Test") == false)
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa" );

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } //make the auto-generated Program accessible programmatically