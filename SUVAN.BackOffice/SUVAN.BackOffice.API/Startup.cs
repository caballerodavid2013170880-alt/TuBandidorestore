using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Auth.Token;
using SUVAN.BackOffice.Service.MensajeriaService;
using System.Text;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

            //  Registro de configuración de servicios
            services.Configure<MailSettingsOptions>(Configuration.GetSection("MailSettings"));
            services.Configure<UnlimitPaySettingsOptions>(Configuration.GetSection("UnlimitPaySettings"));
            services.Configure<PayPalSettingsOptions>(Configuration.GetSection("PayPalSettingsOptions"));
            services.Configure<GlobalConfigsOptions>(Configuration.GetSection("GlobalConfigs"));

            //  Registro correcto de IMarcaService y verificación de inicialización
            services.AddScoped<IMarcaService, MarcaService>();
            Console.WriteLine("IMarcaService registrado correctamente.");

            //  Registro correcto de IModeloService y verificación de inicialización
            services.AddScoped<IModeloService, ModeloService>();
            Console.WriteLine("IModeloService registrado correctamente.");

            //  Configuración de autenticación JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
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
            services.AddDbContext<SuvanDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddHttpContextAccessor();
            services.AddControllers();

            //  Configuración de CORS mejorada
            services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", builder =>
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowed((hosts) => true));
            });

            //  Configuración de Swagger mejorada
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "SUVAN BackOffice API", Version = "V1" });
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

            //  Inicialización de servicios adicionales
            services.AddSignalR();
            services.AddOptions();
            services.AddEndpointsApiExplorer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
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

            //  Activación de Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            //  Mensaje de inicialización para depuración
            logger.LogInformation("Aplicación inicializada correctamente.");
        }
    }
}