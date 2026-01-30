using System.ComponentModel;
using System.Diagnostics;
using ContactsManagerWeb.Filters.ActionFilters;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using LicenseContext = System.ComponentModel.LicenseContext;
using RepositoryContracts;
using Repositories;
using Serilog;
using Serilog.AspNetCore;
using ILogger = Microsoft.Extensions.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

// builder.Host.ConfigureLogging(loggingProvider =>
// {
//     loggingProvider.ClearProviders();
//     loggingProvider.AddConsole();
// });

builder.Host.UseSerilog((HostBuilderContext context,IServiceProvider services, LoggerConfiguration loggerConfiguration
    ) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
    //read our current app services 

});



builder.Services.AddControllersWithViews(options =>
{
    // options.Filters.Add<ResponseHeaderActionFilter>();
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
    options.Filters.Add(new ResponseHeaderActionFilter(logger,"My-key-global","My-value-global",2));
    
});

// Required for app.UseHttpLogging()
builder.Services.AddHttpLogging(options =>
{
    // Log request/response properties (adjust as needed)
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseHeaders |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});


//add services into IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

// builder.Services.AddDbContext<PersonsDbContext>(options =>
//   options.UseSqlite(
//     builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSql")
    ));



var app = builder.Build();


app.UseSerilogRequestLogging();
// app.Logger.LogDebug("Debug message");
// app.Logger.LogInformation("Information message");
// app.Logger.LogWarning("Warning message");
// app.Logger.LogError("Error message");
// app.Logger.LogCritical("Critical message");

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

// Rotativa looks for the wkhtmltopdf binary under: <webRootPath>/<wkhtmltopdfRelativePath>/wkhtmltopdf
// On macOS, put the executable at: wwwroot/Rotativa/wkhtmltopdf (no .exe)
if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup(
        rootPath: app.Environment.WebRootPath,
        wkhtmltopdfRelativePath: "Rotativa");
}


app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();


public partial class Program{ }

