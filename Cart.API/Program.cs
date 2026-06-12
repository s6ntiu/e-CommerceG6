using Serilog;
using ECommerce.Shared.Middleware;
using ECommerce.Shared.HealthChecks;
using ECommerce.Shared.ExceptionHandlers;
using Cart.API.Data;
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

// Registrar Repository e Initializer
builder.Services.AddSingleton<Cart.API.Data.DatabaseInitializer>();
builder.Services.AddScoped<Cart.API.Data.CartRepository>();
void ConfigureProductsClient(HttpClient client) {
    client.BaseAddress = new Uri("http://localhost:5000/");
}
builder.Services.AddHttpClient("ProductsAPI", ConfigureProductsClient);

void ConfigureUsersClient(HttpClient client) {
    client.BaseAddress = new Uri("http://localhost:5000/");
}
builder.Services.AddHttpClient("UsersAPI", ConfigureUsersClient);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
builder.Services.AddExceptionHandler<UnprocessableEntityExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddCheck<ApiStatusCheck>("api_status")
    .AddCheck<SqliteHealthCheck>("sqlite_status");



builder.Services.AddHttpClient("UsersAPI", client =>
{
    // Cambia el puerto según corresponda en tu entorno
    client.BaseAddress = new Uri("https://localhost:7000/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

// Mapear los endpoints
Cart.API.Endpoints.CartEndpoints.MapCartEndpoints(app);

app.Run();