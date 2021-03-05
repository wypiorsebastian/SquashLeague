using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SquashLeague.Application;
using SquashLeague.Infrastructure;
using SquashLeague.Persistence;

namespace SquashLeague.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("SquashLeagueSpec",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Squash League Api Info",
                        Version = "1",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {
                            Email = "wypior.sebastian@gmail.com",
                            Name = "Sebasian Wypiór"
                        }
                    });
            });

            services.AddControllers();
            services.AddPersistenceServices(Configuration);
            services.AddInfrastructureServices();
            services.AddApplicationServices();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/SquashLeagueSpec/swagger.json", "MyApi");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.UseCors("Open");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
