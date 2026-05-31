using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Products.API.Data;
using Products.API.Extensions;
using Products.API.HealthChecks;
using ECommerce.Shared.Middleware;
using ECommerce.Shared.HealthChecks;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddHealthChecks().AddCheck<ApiStatusCheck>("api_status").AddCheck<SqliteHealthCheck>("sqlite_status");
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseMiddleware<AuditMiddleware>();
using (var scope = app.Services.CreateScope()) { var dbInit = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>(); dbInit.Initialize(); }
app.MapProductEndpoints();
app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
