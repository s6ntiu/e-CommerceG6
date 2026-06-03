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
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
builder.Services.AddExceptionHandler<UnprocessableEntityExceptionHandler>();

builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<OrderRepository>();

builder.Services.AddHttpClient("UsersAPI", c => c.BaseAddress = new Uri("http://localhost:5002/"));
builder.Services.AddHttpClient("ProductsAPI", c => c.BaseAddress = new Uri("http://localhost:5001/"));
builder.Services.AddHttpClient("CartAPI", c => c.BaseAddress = new Uri("http://localhost:5003/"));
builder.Services.AddHttpClient("NotificationsAPI", c => c.BaseAddress = new Uri("http://localhost:5004/"));

builder.Services.AddHealthChecks()
    .AddCheck<ApiStatusCheck>("api_status")
    .AddCheck<SqliteHealthCheck>("sqlite_status");

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