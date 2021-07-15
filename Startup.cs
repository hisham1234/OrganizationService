using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Organization_Service.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Organization_Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        // enabled CORS policy for Crisys-UI
        // http://localhost:4200 should be removed later in the production
            services.AddCors(options =>
           {
               options.AddPolicy(name: MyAllowSpecificOrigins,
               builder =>
               {
                builder.WithOrigins("https://crisys-ui.azurewebsites.net", "http://localhost:4200");
            });
           });

            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTSTRING");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            services.AddDbContext<OrganizationContext>(options =>
                options.UseMySql(connectionString, serverVersion).EnableSensitiveDataLogging().EnableDetailedErrors());
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Organization_Service", Version = "v1" });
            });


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Organization_Service v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            //enabled CORS policy
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
