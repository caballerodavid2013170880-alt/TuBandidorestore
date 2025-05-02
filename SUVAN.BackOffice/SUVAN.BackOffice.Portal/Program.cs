using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Portal;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.LogEntidades;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
    .SetApplicationName("AppSuvanWeb");

// Configuración de la cultura global
//var cultureInfo = new CultureInfo("es-MX");
//cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
//cultureInfo.DateTimeFormat.LongTimePattern = "dd/MM/yyyy hh:mm:ss";

//CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
//CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddControllersWithViews()
//    .AddJsonOptions(options =>
//    {
//      options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
//    });

//builder.Services.AddScoped<LoggingInterceptor>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SuvanDbContext>(options =>
{
  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
  //options.AddInterceptors(builder.Services.BuildServiceProvider().GetService<LoggingInterceptor>()!);
});

builder.Services.Configure<MailSettingsOptions>(
 builder.Configuration.GetSection("MailSettings"));

builder.Services.Configure<MFASettingsOptions>(
 builder.Configuration.GetSection("MFASettings"));

builder.Services.Configure<GoogleSettingsOptions>(
 builder.Configuration.GetSection("GoogleSettings"));

builder.Services.Configure<GlobalConfigsOptions>(
 builder.Configuration.GetSection("GlobalConfigs"));

builder.Services.AddAuthentication("AuthScheme")
    .AddCookie("AuthScheme", options =>
    {
      options.ExpireTimeSpan = TimeSpan.FromDays(1);
      options.SlidingExpiration = true;
      options.LoginPath = "/seguridad/login";
      options.LogoutPath = "/seguridad/logout";
      options.AccessDeniedPath = "/seguridad/accesodenegado";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.Name = ".MiAplicacion.Auth";
  options.Cookie.SameSite = SameSiteMode.Strict;
  options.Cookie.HttpOnly = true;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo si usas HTTPS
});

builder.Services.AddAntiforgery(options =>
{
  options.HeaderName = "X-CSRF-TOKEN"; // Opcional, define un encabezado específico
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddDirectoryBrowser();

// inyectamos los servicios
builder.Services.AddCatalogServices();
builder.Services.AddAdminServices();
builder.Services.AddSignalR();
//builder.Services.AddMvc().AddViewOptions(options =>
//{
//  options.HtmlHelperOptions.FormInputRenderMode = Microsoft.AspNetCore.Mvc.Rendering.FormInputRenderMode.AlwaysUseCurrentCulture;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Seguridad/Error");
  // Configurar una página de error personalizada para el código de estado 404.
  app.UseStatusCodePagesWithReExecute("/Seguridad/Error", "?statusCode={0}");

  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
  FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json")),
  RequestPath = "/json"
});

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
