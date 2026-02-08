using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace CRUDTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Test");

        builder.ConfigureServices(sevices =>
        {
          var descripter = sevices.SingleOrDefault(temp =>
                temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
          
          
          builder.ConfigureServices(services => {
              var descripter = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

              if (descripter != null)
              {
                  services.Remove(descripter);
              }
              // Remove the app's real database provider (e.g., Npgsql) registrations
              services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
              services.RemoveAll(typeof(ApplicationDbContext));
              services.AddDbContext<ApplicationDbContext>(options =>
              {
                  options.UseInMemoryDatabase("DatbaseForTesting");
              });
          });
          
          

        });
    }
    
    
}