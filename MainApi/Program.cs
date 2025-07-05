using FastEndpoints;
using FastEndpoints.Swagger;
using Flurl.Http.Configuration;
using MainApi;
using MainApi.Context;
using MainApi.Options;
using MainApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);
var conntectionString = builder.Configuration.GetConnectionString("SomeAppDbConnection");

builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddScoped<ApplicationContext>();

builder.Services.AddDbContext<ApplicationContext>(options => { options.UseNpgsql(conntectionString); });
builder.Services.AddScoped<DatabaseSeeder>();


builder.Services.ConfigureBasicApiServices();
builder.Services.Configure<AuthApiOptions>(builder.Configuration.GetSection("AuthApi"));

builder.Services.AddSingleton<IFlurlClientCache, FlurlClientCache>();

builder.Services.ConfigureAppAuth(builder.Configuration);
builder.Services.AddFastEndpoints();

builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.SwaggerDocument();

var app = builder.Build();
var isDev = app.Environment.IsDevelopment();

app.UseSerilogRequestLogging();
app.UseAppAuth();

app
    .UseFastEndpoints(
        //c => { c.Endpoints.Configurator = epd => epd.AllowAnonymous(); }
    ).UseSwaggerGen();


await app.ApplyPendingMigrations<ApplicationContext>(isDev);
if(isDev)
    await app.SeedDemoData();



app.Run();