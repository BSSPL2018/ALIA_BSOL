using BSOL.Core;
using BSOL.Helpers;
using BSOL.Services;
using BSOL.Web.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using Serilog;
using System.Text;
using Telerik.Reporting.Services;

//https://stackoverflow.com/questions/69722872/asp-net-core-6-how-to-access-configuration-during-setup
//https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-5.0&tabs=visual-studio#where-do-i-put-state-that-was-stored-as-fields-in-my-program-or-startup-class

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
var services = builder.Services;

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

services.AddDbContext<BSOLContext>(x => x.UseSqlServer(configuration.GetValue<string>("ConnectionStrings:BSOL_CONNECTION_STRING")));
services.AddDbContext<BSOLWebContext>(x => x.UseSqlServer(configuration.GetValue<string>("ConnectionStrings:BSOLWEB_CONNECTION_STRING")));
services.AddScoped<AppUser>();
services.AddHostedService<QueuedHostedService>();
services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
services.AddScoped<NotificationService>();
services.AddHttpClient<NotificationService>();
services.AddScoped<ICommonHelper, CommonHelper>();
services.AddHttpContextAccessor();
services.AddTransient<ITicketStore, InMemoryTicketStore>();
services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, ConfigureCookieAuthenticationOptions>();
services.TryAddSingleton<IReportServiceConfiguration, ReportServiceConfiguration>();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(x =>
        {
            x.LoginPath = "/";
            x.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("CookieAuth:Expiry"));
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTAuth:Secret"))),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });


services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
});

// If using Kestrel:
services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

// If using IIS:
services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/").AddPageRoute("/Error", "/Error/{id}").AllowAnonymousToFolder("/Promotion");
});
services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});/* Newtonsoft enabled only for Telerik Reporting */
//.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

services.AddKendo();
services.AddResponseCompression();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});
services.AddMvcCore().AddApiExplorer();
services.AddCors(o => o.AddPolicy("CustomPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();
if (app.Environment.IsDevelopment() || true)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseCors("CustomPolicy");
app.UseAuthentication();

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseResponseCompression();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

ApplicationContext.Configure(app.Services.GetService<IHttpContextAccessor>());

app.Run();
