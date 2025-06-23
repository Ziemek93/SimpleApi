using Serilog;
using UserInteractions.Api;
using UsersInteractions.Application;
using UsersInteractions.Domain.Options;
using UsersInteractions.Infrastructure;
using UsersInteractions.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<AuthApiOptions>(builder.Configuration.GetSection("AuthApi"));

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.ConfigureMassTransit(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.ApplyPendingMigrations<ApplicationContext>();

app.Run();
