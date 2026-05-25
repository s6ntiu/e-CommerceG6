using Microsoft.AspNetCore.Http.HttpResults;
using Products.API.ExceptionHandlers;
using System.Text.Json.Serialization;

// Cambié SlimBuilder por el Builder normal que es el estándar para estos TPs
var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddOpenApi();

// Servicios para que funcionen los controladores del E-Commerce
builder.Services.AddControllers();

// =====================================================================
// 1. ACÁ CONTRATAMOS A TUS GUARDIAS (Siempre antes del builder.Build)
// =====================================================================
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddProblemDetails();
// =====================================================================

var app = builder.Build();

// =====================================================================
// 2. ACÁ LOS PONEMOS A TRABAJAR (Siempre después del builder.Build)
// =====================================================================
app.UseExceptionHandler();
// =====================================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// --- Todo este bloque es el código de ejemplo que ya venía ---
Todo[] sampleTodos =
[
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
];

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos)
        .WithName("GetTodos");
todosApi.MapGet("/{id}", Results<Ok<Todo>, NotFound> (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? TypedResults.Ok(todo)
        : TypedResults.NotFound())
    .WithName("GetTodoById");
// -------------------------------------------------------------

// =====================================================================
// 3. ACTIVAMOS LOS CONTROLADORES (Fundamental para Mariano mañana)
// =====================================================================
app.MapControllers();

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
