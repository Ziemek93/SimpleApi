using AuthApi.Auth;
using AuthApi.Data;
using AuthApi.DependencyInjection;
using AuthApi.Options;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDb");

// Add services to the container.
builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMemoryCache();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AuthConfiguration();
builder.Services.AddSingleton<Token>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<List<M2MOptions>>(builder.Configuration.GetSection("M2MClients"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var isDev = app.Environment.IsDevelopment();

app.UseSerilogRequestLogging(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

await app.ApplyPendingMigrations<ApplicationContext>(isDev);

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }));
app.Run();
