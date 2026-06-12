using Serilog;
using ECommerce.Shared.Middleware;
using ECommerce.Shared.HealthChecks;
using ECommerce.Shared.ExceptionHandlers;
using User.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// --- REGISTRO DE SERVICIOS ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// excepciones globales de lean
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
builder.Services.AddExceptionHandler<UnprocessableEntityExceptionHandler>();
builder.Services.AddProblemDetails();

// Registro de tu servicio de usuarios
builder.Services.AddScoped<User.API.Data.UserRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddHealthChecks()
    .AddCheck<ApiStatusCheck>("api_status")
    .AddCheck<SqliteHealthCheck>("sqlite_status");

var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// usar exception handler global 
void ConfigureExceptionHandler(IApplicationBuilder opt) { }
app.UseExceptionHandler(ConfigureExceptionHandler);
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<AuditMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbInit = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    dbInit.Initialize();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();