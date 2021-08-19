using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Organization_Service.Entities;
using Organization_Service.Helpers;
namespace Organization_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);
            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<OrganizationContext>();
                    context.Database.EnsureCreated();
                    // DbInitializer.Initialize(context);

                    // Data seeding
                    // Add an admin
                    var salt = SaltedHashedHelper.GetSalt();
                    context.User.Add(new UserEntity { 
                        Email = "admin",
                        Password = SaltedHashedHelper.StringEncrypt("admin", salt),
                        FirstName = "admin",
                        LastName = "admin",
                        OfficeID = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Salt = salt
                    });

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
