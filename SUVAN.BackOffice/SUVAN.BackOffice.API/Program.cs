using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.API;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.API
{
  public class Program
  {
    public static void Main(string[] args)
    {

      CreateWebHostBuilder(args).Build().Run();
    }
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          var env = hostingContext.HostingEnvironment;
          config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        })
        .UseStartup<Startup>();
  }
}

