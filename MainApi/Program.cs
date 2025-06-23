using FastEndpoints;
using FastEndpoints.Swagger;
using Flurl.Http.Configuration;
using MainApi.Context;
using MainApi.Extensions;
using MainApi.Options;
using MainApi.Services;
using Microsoft.EntityFrameworkCore;

namespace MainApi;

public partial class Program
{
    static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var conntectionString = builder.Configuration.GetConnectionString("SomeAppDbConnection");
        builder.Services.AddScoped<ApplicationContext>();
        //builder.Services.AddScoped<IAuthServiceOld, AuthServiceOld>();


        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(conntectionString);
        });
        builder.Services.Configure<AuthApiOptions>(builder.Configuration.GetSection("AuthApi"));

        builder.Services.ConfigureBasicApiServices();
        
        builder.Services.AddSingleton<IFlurlClientCache, FlurlClientCache>();
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<JwksService>();
        builder.Services.ConfigureAuth(builder.Configuration);
        builder.Services.ConfigureMassTransit(builder.Configuration);


        //Authentication and Authorization
        var apiKey = builder.Configuration.GetSection("Auth").GetSection("ApiKey").Value;


        builder.Services.SwaggerDocument();

        var app = builder.Build();

        //app.MapGroup("/identity").MapIdentityApi<IdentityUser>();

        app
            .UseAuthentication()
            .UseAuthorization()
            .UseFastEndpoints(
                //c => { c.Endpoints.Configurator = epd => epd.AllowAnonymous(); }
            ).UseSwaggerGen();


        app.ApplyPendingMigrations<ApplicationContext>();
        app.Run();
    }
}