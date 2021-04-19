using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SquashLeague.Application;
using SquashLeague.Infrastructure;
using SquashLeague.Persistence;
using System.Text;
using SquashLeague.Infrastructure.Settings;

namespace SquashLeague.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
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
            services.Configure<JwtSettings>(options =>
                Configuration.GetSection("JwtSettings").Bind(options));

            var issuer = Configuration["JwtSettings:Issuer"];
            var audience = Configuration["JwtSettings:Audience"];
            var key = Configuration["JwtSettings:Key"];
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                //ValidIssuer = issuer,
                //ValidAudience = audience,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("IsPlayer", policy => policy.RequireRole("Player"));
            });
            
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
