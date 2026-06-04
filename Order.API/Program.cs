using ECommerce.Shared.Middleware;
using ECommerce.Shared.HealthChecks;
using ECommerce.Shared.ExceptionHandlers;
using Order.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();

builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddHealthChecks()
    .AddCheck<ApiStatusCheck>("api_status")
    .AddCheck<SqliteHealthCheck>("sqlite_status");

// Registro de clientes HTTP
builder.Services.AddHttpClient("ProductsAPI", client =>
{
    // Cambia el puerto según corresponda en tu entorno
    client.BaseAddress = new Uri("http://localhost:5001/");
});

builder.Services.AddHttpClient("UsersAPI", client =>
{
    // Cambia el puerto según corresponda en tu entorno
    client.BaseAddress = new Uri("http://localhost:5002/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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