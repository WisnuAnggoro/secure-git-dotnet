using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi
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
            services.AddOptions();
            services.Configure<SettingOptions>(Configuration);

            services.AddMvc();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "Jwt";
                options.DefaultChallengeScheme = "Jwt";            
            })
                .AddJwtBearer("Jwt", jwtBearerOptions =>
                {                        
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {                            
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                Configuration["JwtPrivateKey"])),

                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JwtIssuer"],

                        ValidateAudience = true,
                        ValidAudience = Configuration["JwtAudience"],

                        ValidateLifetime = true, //validate the expiration and not before values in the token

                        // ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
