using Serilog;
using Shared.Auth;
using UsersInteractions.Application;
using UsersInteractions.Infrastructure;
using UsersInteractions.Infrastructure.Data;
using UsersInteractions.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, loggerConfig) => 
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddInfrastructureConfiguration(builder.Configuration);
builder.Services.ConfigureMassTransit(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.ConfigureAppAuth(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500", "null") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalRConfigiration();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var isDev = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging(); 
app.UseCors();
if (isDev)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAppAuth();

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");

await app.ApplyPendingMigrations<ApplicationContext>(isDev);

app.Run();
