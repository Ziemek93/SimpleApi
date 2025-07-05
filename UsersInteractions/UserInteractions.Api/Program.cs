using Serilog;
using Shared.Auth;
using UsersInteractions.Application;
using UsersInteractions.Infrastructure;
using UsersInteractions.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.ConfigureMassTransit(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.ConfigureAppAuth(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var isDev = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAppAuth();

app.MapControllers();
await app.ApplyPendingMigrations<ApplicationContext>(isDev);

app.Run();
