using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Auth.Token;
using SUVAN.BackOffice.Service.MensajeriaService;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace SUVAN.BackOffice.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var jwt = Configuration.GetSection("JWTSettings").Get<JWTSettings>();
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.Configure<MailSettingsOptions>(Configuration.GetSection("MailSettings"));
            services.Configure<UnlimitPaySettingsOptions>(Configuration.GetSection("UnlimitPaySettings"));
            services.Configure<PayPalSettingsOptions>(Configuration.GetSection("PayPalSettingsOptions"));
            services.Configure<GlobalConfigsOptions>(Configuration.GetSection("GlobalConfigs"));

            services.AddAuthentication(opttions =>
            {
                opttions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opttions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opttions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuer = jwt.ValidateIssuer,
                    ValidateAudience = jwt.ValidateAudience,
                    ValidateLifetime = jwt.ValidateLifetime,
                    ValidateIssuerSigningKey = jwt.ValidateIssuerSigningKey

                };
                o.MapInboundClaims = false;
            });
            services.AddAuthorization();
            services.AddDbContext<SuvanDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddHttpContextAccessor();
            services.AddControllers(options =>
            {
                options.GeneralRoutePrefix("Api/"); //Colocar el prefix de la aplicacion
            });
            services.AddSignalR();
            services.AddOptions();
            services.AddEndpointsApiExplorer();
            services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "SUVAN BackOffice API ", Version = "V1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = @"JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                      {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference= new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Scheme="oauth2",
                            Name="Bearer",
                            In=ParameterLocation.Header

                        },
                        new List<string>()
                    }

                      });
            });
            // inyectamos los servicios
            services.AddCatalogServices();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CORSPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapHub<HubSuVanService>("/Hub/Mensajeria");
            });
            app.UseSwagger();
            app.UseSwaggerUI();
        }

    }
}
