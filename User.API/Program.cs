using ECommerce.Shared.Middleware;
using ECommerce.Shared.HealthChecks;
using ECommerce.Shared.ExceptionHandlers;
using User.API.Data;
using User.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// --- REGISTRO DE SERVICIOS ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agrega el generador nativo de OpenAPI de .NET 9
builder.Services.AddOpenApi();

// Interceptores de excepciones globales de Leandro
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddProblemDetails();

// Registro de tu servicio de usuarios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddHealthChecks()
    .AddCheck<ApiStatusCheck>("api_status")
    .AddCheck<SqliteHealthCheck>("sqlite_status");

var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE (MIDDLEWARES) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Mapea el endpoint nativo de OpenAPI (localhost:5028/openapi/v1.json)
    app.MapOpenApi();
}

// Activa el manejador global de errores
app.UseExceptionHandler(opt => { });
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