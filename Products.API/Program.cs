using ECommerce.Shared.ExceptionHandlers; // Tus guardias compartidos
using Serilog; // El paquete de logs que acabás de instalar

// 1. ARRANCAMOS SERILOG ANTES QUE NADA
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando Products.API...");

    var builder = WebApplication.CreateBuilder(args);

    // 2. CONFIGURAMOS SERILOG EN EL BUILDER (Guarda logs en consola y en un archivo de texto)
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/products-log-.txt", rollingInterval: RollingInterval.Day));

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // 3. CONTRATAMOS A TUS GUARDIAS (Desde ECommerce.Shared)
    builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
    builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
    builder.Services.AddProblemDetails();

    // 4. AGREGAMOS EL SERVICIO DE HEALTH CHECKS
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // 5. PONEMOS A TRABAJAR A LOS GUARDIAS
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MapControllers();

    // 6. ACTIVAMOS LAS RUTAS DE HEALTH CHECK (Para cumplir con la pág 16 del PDF)
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready");
    app.MapHealthChecks("/health/live");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación Products.API falló al iniciar de forma catastrófica.");
}
finally
{
    Log.CloseAndFlush();
}