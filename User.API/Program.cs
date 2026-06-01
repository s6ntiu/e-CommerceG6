using ECommerce.Shared.ExceptionHandlers;
using User.API.Services;

var builder = WebApplication.CreateBuilder(args);

// --- REGISTRO DE SERVICIOS ---
builder.Services.AddControllers();

// Agrega el generador nativo de OpenAPI de .NET 9
builder.Services.AddOpenApi();

// Interceptores de excepciones globales de Leandro
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddProblemDetails();

// Registro de tu servicio de usuarios
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE (MIDDLEWARES) ---
if (app.Environment.IsDevelopment())
{
    // Mapea el endpoint nativo de OpenAPI (localhost:5028/openapi/v1.json)
    app.MapOpenApi();
}

// Activa el manejador global de errores de Leandro
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();