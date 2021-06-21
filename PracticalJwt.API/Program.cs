using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PracticalJwt.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://*:5050", "http://*:5051");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
